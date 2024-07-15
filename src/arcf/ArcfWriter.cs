namespace arcf
{
    //Class used for writing to streams made for creating arcf files
    public class ArcfWriter : IDisposable
    {
        public Stream OutputStream
        {
            get => _outputStream;
        }

        private readonly Stream _outputStream;

        private readonly List<string> relativeDirectoryPaths = new();

        /// <summary>
        /// Key is relative file path, value is stream
        /// </summary>
        private readonly Dictionary<string, Stream> files = new();

        private bool isDisposed = false;

        public ArcfWriter(Stream stream)
        {
            _outputStream = stream;

            if (!_outputStream.CanWrite)
                throw new Exception("[ArcfWriter] Can not write to ArcfWriter's output stream!");
        }

        public void AddStream(Stream stream, string toRelativePath)
        {
            if (isDisposed)
                throw new Exception("[ArcfWriter] ArcfWriter has been disposed!");

            if (!stream.CanRead)
                throw new Exception("[ArcfWriter] Can not read stream!");

            //add relative directory if necessary

            string? relativeDirectory = Path.GetDirectoryName(toRelativePath);

            if (relativeDirectory != null && !relativeDirectoryPaths.Contains(relativeDirectory))
                AddRelativeDirectoryPath(relativeDirectory);

            //add relative path and stream
            files.Add(toRelativePath, stream);

#if DEBUG
            Console.WriteLine("[ArcfWriter] Added stream to: " + toRelativePath);
#endif

        }

        public void AddRelativeDirectoryPath(string relativeDirectoryPath)
        {
            relativeDirectoryPaths.Add(Path.TrimEndingDirectorySeparator(relativeDirectoryPath));
#if DEBUG
            Console.WriteLine("[ArcfWriter] Added relative directory: " + Path.TrimEndingDirectorySeparator(relativeDirectoryPath));
#endif
        }

        /// <summary>
        /// Writes the output to the stream
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public void WriteTo()
        {
            if (isDisposed)
                throw new Exception("[ArcfWriter] ArcfWriter has been disposed!");

            if (!_outputStream.CanWrite)
                throw new Exception("[ArcfWriter] Can not write to ArcfWriter's stream!");

            //For testing purposes
#if DEBUG
            Console.WriteLine("[ArcfWriter] Opening output stream writer - Test");
#endif

            StreamWriter sw = new(_outputStream);

#if DEBUG
            Console.WriteLine("[ArcfWriter] Writing relative directories - Test");
#endif

            sw.WriteLine("ARCF_EXT_VTEST");
            sw.WriteLine("\n# This is test output!!\n");
            sw.WriteLine("\n|| Relative directories ||\n");

            foreach (string directoryPath in relativeDirectoryPaths)
            {
#if DEBUG
                Console.WriteLine($"[ArcfWriter] Writing relative directory: {directoryPath} - Test");
#endif

                sw.WriteLine(directoryPath);

            }

#if DEBUG
            Console.WriteLine("[ArcfWriter] Writing files (paths only) - Test");
#endif

            sw.WriteLine("\n|| Files ||\n");

            int fileCount = files.Count;
            int filesWritten = 0;

            foreach (string filePath in files.Keys)
            {
#if DEBUG
                Console.WriteLine($"[ArcfWriter] Writing file path: {filePath} ({filesWritten + 1}/{fileCount}) - Test");
#endif

                sw.WriteLine(filePath);

                filesWritten++;

            }

            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                _outputStream.Dispose();

                foreach (Stream stream in files.Values)
                    stream.Dispose();

                isDisposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
