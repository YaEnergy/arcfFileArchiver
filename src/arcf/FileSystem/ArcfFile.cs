namespace arcf
{
    public struct ArcfFile
    {
        public string Name;

        public long StartDataPosition = 0;
        public long DeflatedDataLength = 0;
        public long FullDataLength = 0;

        public ArcfFile(string name, long startDataPosition, long deflatedDataLength, long fullDataLength)
        {
            Name = name;
            StartDataPosition = startDataPosition;
            DeflatedDataLength = deflatedDataLength;
            FullDataLength = fullDataLength;
        }
    }
}
