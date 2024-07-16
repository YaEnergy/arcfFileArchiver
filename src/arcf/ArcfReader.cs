namespace arcf
{
    //TODO: ArcfReader & ArcfWriter act more like Decoders & Encoders rather than Readers & Writers, change their name

    public class ArcfReader : IDisposable
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

        public string[] GetLowestLevelDirectories()
        {
            //For testing purposes
            StreamReader streamReader = new(_stream);

            //Skip the first 7 lines
            for (int i = 0; i < 7; i++)
                streamReader.ReadLine();

            List<string> lowestLevelDirectories = new();

            while (!streamReader.EndOfStream)
            {
                string? directoryPath = streamReader.ReadLine();

                //a line break is placed at the end of the relative folder paths, when this is reached stop searching for folders
                if (directoryPath == null || directoryPath == "")
                    break;

                lowestLevelDirectories.Add(directoryPath);
            }

            return lowestLevelDirectories.ToArray();
        }

        public string[] GetFiles()
        {
            throw new NotImplementedException();
        }

        public Stream OpenFile(string path)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _stream.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
