﻿namespace arcf
{
    public class ArcfDearchiver : IDisposable
    {
        public ArcfReader ArcfReader
        {
            get => _arcfReader;
        }

        private ArcfReader _arcfReader;

        public ArcfDearchiver(ArcfReader arcfReader)
        {
            _arcfReader = arcfReader;
        }

        public static void Dearchive(ArcfReader arcfReader, string outputPath)
        {
            Console.WriteLine($"[ArcfDearchiver] Extracting to: {outputPath}...");

            ExtractDirectories(arcfReader, outputPath);
            
            ExtractFiles(arcfReader, outputPath);

            Console.WriteLine($"[ArcfDearchiver] Finished extracting to: {outputPath}");
        }

        private static void ExtractDirectories(ArcfReader arcfReader, string outputPath)
        {
            Console.WriteLine($"[ArcfDearchiver] Getting directories...");
            string[] arcfDirectoryPaths = arcfReader.GetLowestLevelDirectories();

            Console.WriteLine($"[ArcfDearchiver] Extracting directories to: {outputPath}...");
            foreach (string arcfDirectoryPath in arcfDirectoryPaths)
            {
                string newPath = Path.TrimEndingDirectorySeparator(outputPath) + @"\" + arcfDirectoryPath;

#if DEBUG
                Console.WriteLine($"[ArcfDearchiver] Extracting DIRECTORY {arcfDirectoryPath} to: {newPath}");
#endif

                Directory.CreateDirectory(newPath);
            }
        }

        private static void ExtractFiles(ArcfReader arcfReader, string outputPath)
        {
            Console.WriteLine($"[ArcfDearchiver] Getting files...");
            string[] arcfFilePaths = arcfReader.GetFiles();

            Console.WriteLine($"[ArcfDearchiver] Extracting files to: {outputPath}...");
            foreach (string arcfFilePath in arcfFilePaths)
            {
                ExtractFile(arcfReader, arcfFilePath, outputPath);
            }
        }

        private static void ExtractFile(ArcfReader arcfReader, string arcfFilePath, string outputPath)
        {
            string newPath = Path.TrimEndingDirectorySeparator(outputPath) + @"\" + arcfFilePath;
#if DEBUG
            Console.WriteLine($"[ArcfDearchiver] Extracting FILE {arcfFilePath} to: {newPath}");
#endif

            FileStream fileStream = File.Create(newPath);

            Stream arcfFileStream = arcfReader.OpenFile(arcfFilePath);

            arcfFileStream.CopyTo(fileStream);

            arcfFileStream.Dispose();

            fileStream.Close();
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
