namespace arcf
{
    public class ArcfDirectory
    {
        public string Name;

        public readonly List<ArcfDirectory> Subdirectories = new();

        public readonly List<ArcfFile> Files = new();

        public ArcfDirectory(string name)
        {
            Name = name;
        }

        public static ArcfDirectory FromPath(string path)
        {
            string[] directoryNames = path.Split('\\');

            if (directoryNames.Length == 0)
                throw new Exception("No directory names in path");

            ArcfDirectory root = new(directoryNames[0]);
            ArcfDirectory directory = root;

            for (int i = 1; i < directoryNames.Length; i++)
            {
                ArcfDirectory subdirectory = new(directoryNames[i]);
                directory.Subdirectories.Add(subdirectory);
                directory = subdirectory;
            }

            return root;
        }
    }
}
