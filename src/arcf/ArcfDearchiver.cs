namespace arcf
{
    public class ArcfDearchiver : IDisposable
    {
        public ArcfDecoder ArcfReader
        {
            get => _arcfReader;
        }

        private ArcfDecoder _arcfReader;

        public ArcfDearchiver(ArcfDecoder arcfReader)
        {
            _arcfReader = arcfReader;
        }

        public static void Dearchive(ArcfDecoder arcfReader, string outputPath)
        {
            Console.WriteLine($"[ArcfDearchiver] Extracting to: {outputPath}...");

            //Extract root directories
#if DEBUG
            Console.WriteLine($"[ArcfDearchiver] Extracting ROOT DIRECTORIES to: {outputPath}...");
#endif
            foreach (ArcfDirectory rootDirectory in arcfReader.GetRootDirectories())
                ExtractDirectory(rootDirectory, arcfReader, outputPath);

            //Extract root files
#if DEBUG
            Console.WriteLine($"[ArcfDearchiver] Extracting ROOT FILES to: {outputPath}...");
#endif
            foreach (ArcfFile rootFile in arcfReader.GetRootFiles())
                ExtractFile(rootFile, arcfReader, outputPath);

            Console.WriteLine($"[ArcfDearchiver] Finished extracting to: {outputPath}");
        }

        private static void ExtractDirectory(ArcfDirectory directory, ArcfDecoder arcfReader, string outputPath)
        {
            string newPath = Path.TrimEndingDirectorySeparator(outputPath) + @"\" + directory.Name;
#if DEBUG
            Console.WriteLine($"[ArcfDearchiver] Extracting DIRECTORY {directory.Name} to: {newPath}");
#endif

            //Create directory if it doesn't exist
            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);

            //Extract subdirectories
#if DEBUG
            Console.WriteLine($"[ArcfDearchiver] Extracting SUB DIRECTORIES to: {newPath}...");
#endif
            foreach (ArcfDirectory subDirectory in directory.Subdirectories)
                ExtractDirectory(subDirectory, arcfReader, newPath);

            //Extract directory files
#if DEBUG
            Console.WriteLine($"[ArcfDearchiver] Extracting DIRECTORY FILES to: {newPath}...");
#endif
            foreach (ArcfFile rootFile in directory.Files)
                ExtractFile(rootFile, arcfReader, outputPath);
        }

        private static void ExtractFile(ArcfFile file, ArcfDecoder arcfReader, string outputPath)
        {
            string newPath = Path.TrimEndingDirectorySeparator(outputPath) + @"\" + file.Name;
#if DEBUG
            Console.WriteLine($"[ArcfDearchiver] Extracting FILE {file.Name} to: {newPath}");
#endif

            FileStream fileStream = File.Create(newPath);

            arcfReader.CopyFileTo(file, fileStream);

            fileStream.Flush();
            fileStream.Dispose();
        }

        public void Dearchive(string outputPath)
        {
            Dearchive(_arcfReader, outputPath);
        }

        public void Dispose()
        {
            _arcfReader.Dispose();

            GC.SuppressFinalize(this);
        }

    }
}
