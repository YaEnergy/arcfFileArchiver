namespace arcf
{
    public class ArcfDearchiver
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

        public static void Dearchive(ArcfReader arcfReader)
        {
            throw new NotImplementedException();
        }

        public void Dearchive()
        {
            Dearchive(_arcfReader);
        }

    }
}
