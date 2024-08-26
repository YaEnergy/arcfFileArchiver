namespace arcf
{
    public class ArcfArchiver : IDisposable
    {
        public ArcfWriter ArcfWriter
        {
            get => _arcfWriter;
        }

        private readonly ArcfWriter _arcfWriter;

        private bool isDisposed = false;

        public ArcfArchiver(Stream stream)
        {
            _arcfWriter = new(stream);
        }

        public ArcfArchiver(ArcfWriter arcfWriter)
        {
            _arcfWriter = arcfWriter;
        }

        public void Archive()
        {
            _arcfWriter.Close();
            _arcfWriter.Dispose();

            isDisposed = true;
        }

        #region Adding files

        public void AddFile(FileInfo file)
        {
            if (isDisposed)
                throw new Exception("[ArcfArchiver] ArcfArchiver has been disposed!");

            if (!file.Exists)
                throw new FileNotFoundException("[ArcfArchiver] File does not exist!", file.FullName);

#if DEBUG
            Console.WriteLine($"[ArcfArchiver] Adding file {file.Name} to {_arcfWriter.CurrentDirectory}");
#endif

            FileStream fileStream = file.OpenRead();
            _arcfWriter.WriteFileStream(file.Name, fileStream);
            fileStream.Dispose();
        }

        public void AddFile(string filePath)
        {
            AddFile(new FileInfo(filePath));
        }

        #endregion

        #region Adding directories

        public void AddDirectory(DirectoryInfo directory, bool recursive = true)
        {
            if (isDisposed)
                throw new Exception("[ArcfArchiver] ArcfArchiver has been disposed!");

            if (!directory.Exists)
                throw new DirectoryNotFoundException("[ArcfArchiver] Directory (" + directory.FullName + ") does not exist!");

            Console.WriteLine("[ArcfArchiver] Adding directory " + directory.FullName + "...");

            _arcfWriter.BeginDirectory(directory.Name);

            if (recursive)
            {
                //Add subdirectories
                foreach (DirectoryInfo subdirectory in directory.GetDirectories())
                {
                    AddDirectory(subdirectory, recursive);
                }
            }

            //Add files
            foreach (FileInfo file in directory.GetFiles())
            {
                AddFile(file);
            }

            _arcfWriter.EndDirectory();

        }

        public void AddDirectory(string directoryPath, bool recursive = true)
        {
            AddDirectory(new DirectoryInfo(directoryPath), recursive);
        }

        #endregion

        public void Dispose()
        {
            if (!isDisposed)
                _arcfWriter.Dispose();

            isDisposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
