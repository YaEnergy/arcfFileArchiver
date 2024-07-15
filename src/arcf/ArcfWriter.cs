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
                throw new Exception("Can not write to ArcfWriter's output stream!");
        }

        public void AddStream(Stream stream, string toRelativePath)
        {
            if (isDisposed)
                throw new Exception("ArcfWriter has been disposed!");

            if (!stream.CanRead)
                throw new Exception("Can not read stream!");

            //add relative path and stream
            files.Add(toRelativePath, stream);

            //add relative directory if necessary

            string? relativeDirectory = Path.GetDirectoryName(toRelativePath);

            if (relativeDirectory == null || relativeDirectoryPaths.Contains(relativeDirectory))
                return;

            AddRelativeDirectoryPath(relativeDirectory);
        }

        public void AddRelativeDirectoryPath(string relativeDirectoryPath)
        {
            relativeDirectoryPaths.Add(Path.TrimEndingDirectorySeparator(relativeDirectoryPath));
        }

        /// <summary>
        /// Writes the output to the stream
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public void WriteTo()
        {
            if (isDisposed)
                throw new Exception("ArcfWriter has been disposed!");

            if (!_outputStream.CanWrite)
                throw new Exception("Can not write to ArcfWriter's stream!");

            throw new NotImplementedException();
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
