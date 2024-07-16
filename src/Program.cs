using arcf;
using System.Diagnostics;

namespace arcfFileArchiver
{
    public static class Program
    {
        private static bool wantsToQuit = false;

        private static void Main(string[] args)
        {
            Console.WriteLine("| arcf File Archiver |\n");
            Console.WriteLine("Write help to show commands\n");

            while (!wantsToQuit)
            {
                string? commandLine = Console.ReadLine();

                if (commandLine == null)
                    continue;

                int commandEndIndex = commandLine.IndexOf(' ');

                if (commandEndIndex == -1)
                    commandEndIndex = commandLine.Length;

                string command = commandLine[..commandEndIndex];

                string[] arguments = commandEndIndex == commandLine.Length ? [] : commandLine.Remove(0, commandEndIndex + 1).Split(' ');

                Console.WriteLine(" ");

                try
                {
                    HandleCommand(command, arguments);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] {ex.Message}");
                }
            }
        }

        private static void HandleCommand(string command, string[] parameters)
        {
            switch (command)
            {
                case "read":
                    Console.WriteLine(command);

                    foreach (string param in parameters)
                        Console.WriteLine(param);

                    break;
                case "archive":
                    {
                        switch(parameters.Length)
                        {
                            case 0:
                                throw new ArgumentException("[Archive] No output file path given!");
                            case 1:
                                throw new ArgumentException("[Archive] No files/directories to archive given!");
                            default:
                                break;
                        }

                        string outputPath = parameters[0];
                        List<string> paths = parameters.ToList();
                        paths.RemoveAt(0);

                        ArchiveCommand(outputPath, paths);

                        break;
                    }
                case "dearchive":
                    {
                        switch (parameters.Length)
                        {
                            case 0:
                                throw new ArgumentException("[Archive] No archive file path given!");
                            case 1:
                                throw new ArgumentException("[Archive] No output path given!");
                            default:
                                break;
                        }

                        string archiveFilePath = parameters[0];
                        string outputPath = parameters[1];

                        DearchiveCommand(archiveFilePath, outputPath);

                        break;
                    }
                case "help":
                    Console.WriteLine("| arcf File Archiver - Help |\n");
                    Console.WriteLine("Read [archive file path] - displays file names & directories in archive file");
                    Console.WriteLine("Archive [output directory] [directories/files to archive] .. - archive files & directories");
                    Console.WriteLine("Dearchive [archive file path] [output directory] - dearchive archive file");
                    Console.WriteLine("Help - shows this menu");
                    Console.WriteLine("Quit - quit the program");
                    Console.WriteLine(" ");
                    break;
                case "quit":
                    Console.WriteLine("Quitting...");

                    wantsToQuit = true;
                    break;
                default:
                    Console.WriteLine($"{command} - is an unknown command!");

                    break;
            }
        }

        private static long GetDirectoryLength(DirectoryInfo directory)
        {
            long length = 0;

            foreach (FileInfo file in directory.GetFiles())
            {
                length += file.Length;
            }

            foreach (DirectoryInfo subdirectory in directory.GetDirectories())
            {
                length += GetDirectoryLength(subdirectory);
            }

            return length;
        }

        private static int GetDirectoryDirectoryCount(DirectoryInfo directory)
        {
            int count = 1; //this directory

            //plus subdirectories
            foreach (DirectoryInfo subdirectory in directory.GetDirectories())
            {
                count += GetDirectoryDirectoryCount(subdirectory);
            }

            return count;
        }

        private static int GetDirectoryFileCount(DirectoryInfo directory)
        {
            int count = 0;

            foreach (FileInfo file in directory.GetFiles())
            {
                count++;
            }

            foreach (DirectoryInfo subdirectory in directory.GetDirectories())
            {
                count += GetDirectoryFileCount(subdirectory);
            }

            return count;
        }

        private static string TextFormatBytes(long bytes)
        {
            const long KB_BYTES = 1024L;
            const long MB_BYTES = KB_BYTES * 1024L;
            const long GB_BYTES = MB_BYTES * 1024L;
            const long TB_BYTES = GB_BYTES * 1024L;

            if (bytes < 10 * KB_BYTES)
                return $"{bytes} bytes";
            else if (bytes < 10 * MB_BYTES)
                return $"{bytes / KB_BYTES} kb";
            else if (bytes < 10 * GB_BYTES)
                return $"{bytes / MB_BYTES} mb";
            else if (bytes < 10 * TB_BYTES)
                return $"{bytes / GB_BYTES} gb";
            else
                return $"{bytes / TB_BYTES} tb";
        }

        private static void ReadCommand(string outputPath, IEnumerable<string> compressPaths)
        {

        }

        private static void ArchiveCommand(string outputPath, IEnumerable<string> compressPaths) 
        {
            Console.WriteLine($"[Archive] Checking if {outputPath} exists...");

            if (File.Exists(outputPath))
            {
                Console.WriteLine($"[WARNING] [Archive] File: {outputPath} already exists, continuing this operation will overwrite it.\nContinue? Y/N\n");
                
                string? alreadyExistsAnswer = Console.ReadLine();
                if (alreadyExistsAnswer == null || alreadyExistsAnswer.ToLower() != "y")
                {
                    Console.WriteLine("[Archive] Cancelled archival.");
                    return;
                }
            }
            else
            {
                Console.WriteLine($"[Archive] File: {outputPath} does not exist. No need to overwrite a file.");
            }

            Console.WriteLine($"[Archive] Getting given path info & verifying their existance...");

            long totalBytesToArchive = 0;
            int fileCount = 0;
            int directoryCount = 0;

            foreach (string path in compressPaths)
            {
                if (!File.Exists(path) && !Directory.Exists(path))
                    throw new ArgumentException($"[Archive] File/Directory does not exist!: {path}");
                else if (File.Exists(path))
                {
                    FileInfo file = new(path);

                    totalBytesToArchive += file.Length;
                    fileCount++;

                    Console.WriteLine($"[Archive] FILE: {path} - exists! ({TextFormatBytes(file.Length)}) (1/{fileCount} files | 0/{directoryCount} directories))");
                }
                else if (Directory.Exists(path))
                {
                    DirectoryInfo directory = new(path);

                    long directoryLength = GetDirectoryLength(directory);
                    int directoryFileCount = GetDirectoryFileCount(directory);
                    int directoryDirectoryCount = GetDirectoryDirectoryCount(directory);

                    totalBytesToArchive += directoryLength;
                    fileCount += directoryFileCount;
                    directoryCount += directoryDirectoryCount;

                    Console.WriteLine($"[Archive] DIRECTORY: {path} - exists! ({TextFormatBytes(directoryLength)}) ({directoryFileCount}/{fileCount} files | {directoryDirectoryCount}/{directoryCount} directories)");
                }
            }

            Console.WriteLine($"\n|| ARCHIVE INFO ||\n");
            Console.WriteLine($"Archiving {fileCount} files");
            Console.WriteLine($"Archiving {directoryCount} directories");
            Console.WriteLine($"Archiving {TextFormatBytes(totalBytesToArchive)}");
            Console.WriteLine($"\nContinue? Y/N\n");

            string? answer = Console.ReadLine();
            if (answer == null || answer.ToLower() != "y")
            {
                Console.WriteLine("[Archive] Cancelled archival.");
                return;
            }

            Console.WriteLine($"[Archive] Opening {outputPath} for writing...");

            FileStream fileStream = File.Create(outputPath);
            ArcfArchiver arcfArchiver = new();

            Console.WriteLine("[Archive] Starting archive...\n");

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                Console.WriteLine("[Archive] Adding files & directories...\n");

                foreach (string path in compressPaths)
                {
                    if (File.Exists(path))
                        arcfArchiver.AddTopLevelFile(path);
                    else if (Directory.Exists(path))
                        arcfArchiver.AddTopLevelDirectory(path);
                    else
                        throw new ArgumentException($"[Archive] File/Directory does not exist!: {path}");
                }

                Console.WriteLine($"[Archive] Writing to: {outputPath}\n");

                arcfArchiver.Archive(fileStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] [Archive] {ex.Message})");

                Console.WriteLine("[Archive] Disposing ArcfArchiver...\n");

                arcfArchiver.Dispose();
                fileStream.Dispose();

                //get rid of possibly corrupted file
                if (File.Exists(outputPath))
                {
                    Console.WriteLine("[Archive] Deleting possibly corrupted archive file.\n");
                    File.Delete(outputPath);
                }

                stopwatch.Stop();

                Console.WriteLine($"[Archive] Finished with an exception. ({stopwatch.Elapsed})\n");
                return;
            }

            Console.WriteLine("[Archive] Disposing ArcfArchiver...\n");

            arcfArchiver.Dispose();
            fileStream.Dispose();

            stopwatch.Stop();

            Console.WriteLine($"[Archive] Finished successfully. ({stopwatch.Elapsed})\n");
        }

        private static void DearchiveCommand(string decompressPath, string outputPath)
        {
            Console.WriteLine($"[Dearchive] Checking if ARCHIVE FILE PATH: {decompressPath} exists...");
            if (!File.Exists(decompressPath))
            {
                throw new ArgumentException("[Dearchive] File/Directory does not exist!: {path}");
            }

            Console.WriteLine($"[Dearchive] Opening {decompressPath} for reading...");
            ArcfReader arcfReader = new(File.OpenRead(decompressPath));

            //TODO: Write some info about the archive file here

            Console.WriteLine("[Dearchive] Start dearchival? Y/N");
            string? dearchiveAnswer = Console.ReadLine();
            if (dearchiveAnswer == null || dearchiveAnswer.ToLower() != "y")
            {
                Console.WriteLine("[Dearchive] Cancelled dearchival.");
                return;
            }

            Console.WriteLine($"[Dearchive] Checking if {outputPath} exists...");

            if (Directory.Exists(outputPath))
            {
                Console.WriteLine($"[Dearchive] OUTPUT DIRECTORY: {outputPath} - exists!");

                DirectoryInfo outputDirectoryInfo = new(outputPath);
                if (outputDirectoryInfo.GetFileSystemInfos().Length > 0)
                {
                    Console.WriteLine($"[Warning] [Dearchive] DIRECTORY: {outputPath} already contains files/directories, and will overwrite them.\nContinue? Y/N");
                    
                    string? fileAnswer = Console.ReadLine();
                    if (fileAnswer == null || fileAnswer.ToLower() != "y")
                    {
                        Console.WriteLine("[Dearchive] Cancelled dearchival.");
                        return;
                    }
                }
            }
            else
            { 
                Console.WriteLine($"[Dearchive] Creating OUTPUT DIRECTORY: {outputPath}");
                Directory.CreateDirectory(outputPath);
            }

            ArcfDearchiver arcfDearchiver = new(arcfReader);

            Console.WriteLine($"[Dearchive] Starting dearchival to DIRECTORY: {outputPath}...\n");

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                arcfDearchiver.Dearchive(outputPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] [Dearchive] {ex.Message})");

                Console.WriteLine("[Derchive] Disposing ArcfDearchiver...\n");

                arcfDearchiver.Dispose();

                //get rid of possibly corrupted file
                /*if (File.Exists(outputPath))
                {
                    Console.WriteLine("[Archive] Deleting possibly corrupted archive file.\n");
                    File.Delete(outputPath);
                }*/

                stopwatch.Stop();

                Console.WriteLine($"[Dearchive] Finished with an exception. ({stopwatch.Elapsed})\n");
                return;
            }

            Console.WriteLine($"[Dearchive] Disposing ArcfDearchiver...");
            arcfDearchiver.Dispose();

            stopwatch.Stop();

            Console.WriteLine($"[Dearchive] Finished successfully. ({stopwatch.Elapsed})\n");
        }
    }
}
