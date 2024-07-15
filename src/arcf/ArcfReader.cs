namespace arcf
{
    public class ArcfReader
    {
        public Stream Stream
        {
            get => _stream;
        }

        private readonly Stream _stream;

        public ArcfReader(Stream stream)
        {
            _stream = stream;
        }

        public string[] GetDirectories()
        {
            throw new NotImplementedException();
        }

        public string[] GetFiles()
        {
            throw new NotImplementedException();
        }

        public Stream OpenFile(string path)
        {
            throw new NotImplementedException();
        }
    }
}
