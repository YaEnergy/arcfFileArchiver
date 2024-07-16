namespace arcf
{
    public struct ArcfFile
    {
        public string Name;

        public long StartDataPosition = 0;
        public long DataLength = 0;

        public ArcfFile(string name, long startDataPosition, long dataLength)
        {
            Name = name;
            StartDataPosition = startDataPosition;
            DataLength = dataLength;
        }
    }
}
