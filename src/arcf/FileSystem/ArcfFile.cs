namespace arcf
{
    public class ArcfFile
    {
        public string Name;

        public Stream? Stream = null;

        public ArcfFile(string name)
        {
            Name = name;
        }

        public ArcfFile(string name, Stream stream)
        {
            Name = name;
            Stream = stream;
        }
    }
}
