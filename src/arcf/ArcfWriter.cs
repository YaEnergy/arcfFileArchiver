
using System.IO;
using System.Xml.Linq;

namespace arcf
{
    public class ArcfWriter : IDisposable
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

        public Stream Stream
        {
            get => _outstream;
        }

        public string CurrentDirectory
        {
            get => directoryStack.Count == 0 ? "arcfRoot" : directoryStack.Peek();
        }

        private readonly Stream _outstream;
        private readonly BinaryWriter writer;

        private Stack<string> directoryStack = new();

        private bool isDisposed = false;

        public ArcfWriter(Stream stream)
        {
            _outstream = stream;
            writer = new BinaryWriter(stream);

            InitArcfStream();
        }

        public void WriteFileStream(string name, Stream stream)
        {
            if (isDisposed)
                throw new Exception("[ArcfWriter] ArcfWriter has been disposed!");

            if (!_outstream.CanWrite)
                throw new Exception("[ArcfWriter] Can not write to output stream!");

            Console.WriteLine($"[ArcfWriter] Writing FILE {name} to {CurrentDirectory}");

            if (stream.Length > (long)int.MaxValue)
                throw new Exception($"[ArcfWriter] FILE {name} is too large! ({stream.Length} bytes > {int.MaxValue} bytes (Int32 max)");

            //Start file
            writer.Write("\\f");

            // string name - file name
            writer.Write(name);

            // long length - length of the data in bytes
            writer.Write(stream.Length);

            // data
            stream.Position = 0;

            //Write stream to decoder file stream
            byte[] buffer = new byte[(int)stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);
            _outstream.Write(buffer, 0, buffer.Length);

            stream.Flush();
            _outstream.Flush();

            Console.WriteLine($"[ArcfWriter] Writed FILE {name} to {CurrentDirectory} ({stream.Length} file bytes)");
        }

        public void BeginDirectory(string name)
        {
            if (isDisposed)
                throw new Exception("[ArcfWriter] ArcfWriter has been disposed!");

            if (!_outstream.CanWrite)
                throw new Exception("[ArcfWriter] Can not write to output stream!");

            //Start directory
            writer.Write("\\d");
            writer.Write(name);

            //Info
            Console.WriteLine($"[ArcfWriter] Started DIRECTORY {name} in {CurrentDirectory}");

            directoryStack.Push(name);
        }

        public void EndDirectory()
        {
            if (isDisposed)
                throw new Exception("[ArcfWriter] ArcfWriter has been disposed!");

            if (!_outstream.CanWrite)
                throw new Exception("[ArcfWriter] Can not write to output stream!");

            //End directory
            writer.Write("\\ed");

            //Info
            string endedDirectoryName = directoryStack.Pop();
            Console.WriteLine($"[ArcfWriter] Ended DIRECTORY {endedDirectoryName} in {CurrentDirectory}");
        }

        private void InitArcfStream()
        {
            if (isDisposed)
                throw new Exception("[ArcfWriter] ArcfWriter has been disposed!");

            if (!_outstream.CanWrite)
                throw new Exception("[ArcfWriter] Can not write to output stream!");

#if DEBUG
            Console.WriteLine("[ArcfWriter] Init Arcf Stream - string extension ARCF_EXT | uint32 version 1");
#endif

            //string extension - String used for identifying arcf files
            writer.Write("ARCF_EXT");

            //uint32 version - Arcf file version number
            writer.Write(1u);
        }

        public void Close()
        {
            writer.Write("\\eaf");

            writer.Flush();

            isDisposed = true;

            writer.Dispose();
            _outstream.Dispose();
        }

        public void Dispose()
        {
            if (!isDisposed)
                Close();

            GC.SuppressFinalize(this);
        }
    }
}
