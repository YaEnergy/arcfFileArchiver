namespace arcf
{
    //Class used for writing to streams made for creating arcf files
    public class ArcfEncoder : IDisposable
    {
        private readonly List<string> relativeDirectoryPaths = new();

        /// <summary>
        /// Key is relative file path, value is stream
        /// </summary>
        private readonly Dictionary<string, Stream> files = new();

        private bool isDisposed = false;

        public ArcfEncoder()
        {
            
        }

        public void AddStream(Stream stream, string toRelativePath)
        {
            if (isDisposed)
                throw new Exception("[ArcfEncoder] ArcfEncoder has been disposed!");

            if (!stream.CanRead)
                throw new Exception("[ArcfEncoder] Can not read stream!");

            //add relative directory if necessary

            string? relativeDirectory = Path.GetDirectoryName(toRelativePath);

            if (relativeDirectory != null && !relativeDirectoryPaths.Contains(relativeDirectory))
                AddRelativeDirectoryPath(relativeDirectory);

            //add relative path and stream
            files.Add(toRelativePath, stream);

#if DEBUG
            Console.WriteLine("[ArcfEncoder] Added stream to: " + toRelativePath);
#endif

        }

        public void AddRelativeDirectoryPath(string relativeDirectoryPath)
        {
            //Remove higher-level directories, only keep the lowest-level directories as they contain the higher-level directory paths aswell (C:\abc\def contains C:\abc)
            foreach (string path in relativeDirectoryPaths)
            {
                if (relativeDirectoryPath.StartsWith(path))
                {
                    relativeDirectoryPaths.Remove(path);
#if DEBUG
                    Console.WriteLine("[ArcfEncoder] Removed higher-level relative directory: " + Path.TrimEndingDirectorySeparator(path));
#endif
                    break;
                }
            }

            relativeDirectoryPaths.Add(Path.TrimEndingDirectorySeparator(relativeDirectoryPath));
#if DEBUG
            Console.WriteLine("[ArcfEncoder] Added relative directory: " + Path.TrimEndingDirectorySeparator(relativeDirectoryPath));
#endif
        }

        /// <summary>
        /// Writes the output to the stream
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public void Encode(Stream outputStream)
        {
            if (isDisposed)
                throw new Exception("[ArcfEncoder] ArcfEncoder has been disposed!");

            if (!outputStream.CanWrite)
                throw new Exception("[ArcfEncoder] Can not write to output stream!");

            //For testing purposes
#if DEBUG
            Console.WriteLine("[ArcfWriter] Opening output stream writer - Test");
#endif

            StreamWriter sw = new(outputStream);

#if DEBUG
            Console.WriteLine("[ArcfEncoder] Writing relative directories - Test");
#endif

            sw.WriteLine("ARCF_EXT_VTEST");
            sw.WriteLine("\n# This is test output!!\n");
            sw.WriteLine("\n|| Relative directories ||\n");

            foreach (string directoryPath in relativeDirectoryPaths)
            {
#if DEBUG
                Console.WriteLine($"[ArcfEncoder] Writing relative directory: {directoryPath} - Test");
#endif

                sw.WriteLine(directoryPath);
            }

#if DEBUG
            Console.WriteLine("[ArcfEncoder] Writing files (paths only) - Test");
#endif

            sw.WriteLine("\n|| Files ||\n");

            int fileCount = files.Count;
            int filesWritten = 0;

            foreach (string filePath in files.Keys)
            {
#if DEBUG
                Console.WriteLine($"[ArcfEncoder] Writing file path: {filePath} ({filesWritten + 1}/{fileCount}) - Test");
#endif

                sw.WriteLine(filePath);

                filesWritten++;
            }

            Console.WriteLine($"[ArcfEncoder] Finished writing to stream");
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                foreach (Stream stream in files.Values)
                    stream.Dispose();

                isDisposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
