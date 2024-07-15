namespace arcfFileArchiver
{
    public static class Program
    {
        private static bool wantsToQuit = false;

        private static void Main(string[] args)
        {
            Console.WriteLine("| arcf File Archiver |");

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

                HandleCommand(command, arguments);
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
                    Console.WriteLine(command);

                    foreach (string param in parameters)
                        Console.WriteLine(param);

                    break;
                case "dearchive":
                    Console.WriteLine(command);

                    foreach (string param in parameters)
                        Console.WriteLine(param);

                    break;
                case "help":
                    Console.WriteLine("| arcf File Archiver - Help |");
                    Console.WriteLine("Read [archive file path] - displays file names & directories in archive file");
                    Console.WriteLine("Archive [output directory] [directories/files to archive] .. - archive files & directories");
                    Console.WriteLine("Dearchive [archive file path] [output directory] - dearchive archive file");
                    Console.WriteLine("Help - shows this menu");
                    Console.WriteLine("Quit - quit the program");
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

        private static void ReadCommand(string outputPath, IEnumerable<string> compressPaths)
        {

        }

        private static void ArchiveCommand(string outputPath, IEnumerable<string> compressPaths) 
        { 
            
        }

        private static void DearchiveCommand(IEnumerable<string> decompressPaths, string outputPath)
        {

        }
    }
}
