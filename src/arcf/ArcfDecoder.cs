﻿namespace arcf
{
    public class ArcfDecoder : IDisposable
    {
        // # Arcf file layout #

        //string extension - String used for identifying arcf files
        //uint32 version - Arcf file version number

        // Read the next string.

        // \d - start directory
        // followed by:
        // string name - directory name

        // \f - start file

        // followed by:
        // string name - file name
        // long length - length of the data in bytes
        // data

        // \ed - end directory

        // \eaf - end archive file

        const uint READER_ARCF_VERSION = 1;

        public Stream Stream
        {
            get => _stream;
        }

        public long ArchiveSizeBytes
        {
            get => _archiveSizeBytes;
        }

        public int NumFiles
        {
            get => _numFiles;
        }

        public int NumDirectories
        {
            get => _numDirectories;
        }

        private readonly Stream _stream;
        private readonly BinaryReader reader;

        private bool isDisposed = false;

        // # Data
        private uint arcfVersion = 0;
        private readonly ArcfDirectory arcfRoot = new("arcfRoot");
        private long _archiveSizeBytes = 0L;
        private int _numFiles = 0;
        private int _numDirectories = 0;

        private readonly Stack<ArcfDirectory> directoryStack = new();

        public ArcfDecoder(Stream stream)
        {
            _stream = stream;
            reader = new(stream);

            Decode();
        }

        private void Decode()
        {
            VerifyStream();

            Console.WriteLine("[ArcfDecoder] Reading arcf stream...");
            Console.WriteLine("[ArcfDecoder] Reading arcf version...");

            arcfVersion = reader.ReadUInt32();

            Console.WriteLine($"[ArcfDecoder] ARCF VERSION {arcfVersion}");

            if (arcfVersion != READER_ARCF_VERSION)
                throw new Exception($"[ArcfDecoder] Reader is version {READER_ARCF_VERSION}, but file is version {READER_ARCF_VERSION}");

            directoryStack.Push(arcfRoot);

            Console.WriteLine("[ArcfDecoder] Reading arcf files & directories...");

            string command = reader.ReadString();

            ArcfDirectory currentDirectory = arcfRoot;

            //while eaf (end archive file) command isn't given
            while (command != "\\eaf")
            {
                switch(command)
                {
                    //Start directory
                    case "\\d":
                        {
                            _numDirectories++;

                            string newDirectoryName = reader.ReadString();

                            ArcfDirectory newDirectory = new(newDirectoryName);
                            currentDirectory.Subdirectories.Add(newDirectory);
                            directoryStack.Push(newDirectory);

                            currentDirectory = newDirectory;

#if DEBUG
                            Console.WriteLine($"[ArcfDecoder] Start DIRECTORY {newDirectoryName}");
#endif
                            break;
                        }
                    //End directory
                    case "\\ed":
                        {
                            ArcfDirectory endingDirectory = directoryStack.Pop();
                            currentDirectory = directoryStack.Peek();

#if DEBUG
                            Console.WriteLine($"[ArcfDecoder] End DIRECTORY {endingDirectory.Name}, back to DIRECTORY {currentDirectory.Name}");
#endif
                            break;
                        }
                    //Start file
                    case "\\f":
                        {
                            _numFiles++;

                            string fileName = reader.ReadString();

                            long dataLength = reader.ReadInt64();
                            _archiveSizeBytes += dataLength;

                            long startDataPosition = _stream.Position;

                            //Skip data
                            _stream.Position += dataLength;

                            ArcfFile file = new(fileName, startDataPosition, dataLength);
                            currentDirectory.Files.Add(file);

#if DEBUG
                            Console.WriteLine($"[ArcfDecoder] Found FILE {fileName} ({dataLength} bytes)");
#endif
                            break;
                        }
                }

                //Next command
                command = reader.ReadString();
            }

            Console.WriteLine("[ArcfDecoder] Reached end of archive file (\\eaf)");

            Console.WriteLine("[ArcfDecoder] Finished reading archive file");
        }

        private void VerifyStream()
        {
            Console.WriteLine("[ArcfDecoder] Verifying stream...");

            if (isDisposed)
                throw new Exception("[ArcfDecoder] ArcfDecoder has been disposed!");

            if (!_stream.CanRead)
                throw new Exception("[ArcfDecoder] Can not read stream!");

            _stream.Position = 0;
            string extension = reader.ReadString();

            if (extension != "ARCF_EXT")
                throw new Exception("[ArcfDecoder] This is not an arcf stream!");

            Console.WriteLine("[ArcfDecoder] Verified arcf stream!");
        }

        public ArcfDirectory[] GetRootDirectories()
        {
            return arcfRoot.Subdirectories.ToArray();
        }

        public ArcfFile[] GetRootFiles()
        {
            return arcfRoot.Files.ToArray();
        }

        public void CopyFileTo(ArcfFile file, Stream stream)
        {
            if (file.DataLength > (long)int.MaxValue)
                throw new Exception($"[ArcfDecoder] FILE {file.Name} is too large! ({file.DataLength} bytes > {int.MaxValue} bytes (Int32 max)");

            _stream.Position = file.StartDataPosition;
            stream.Position = 0;

            //Write decoder file stream to stream
            byte[] buffer = new byte[(int)file.DataLength];
            _stream.Read(buffer, 0, (int)file.DataLength);
            stream.Write(buffer, 0, buffer.Length);
            
            stream.Flush();
            _stream.Flush();

#if DEBUG
            Console.WriteLine($"[ArcfDecoder] Copied FILE {file.Name} to stream ({(int)file.DataLength} bytes)");
#endif
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                reader.Dispose();
                _stream.Dispose();
            }

            isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
