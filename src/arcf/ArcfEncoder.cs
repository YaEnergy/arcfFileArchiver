using System.IO;

namespace arcf
{
    //Class used for writing to streams made for creating arcf files
    public class ArcfEncoder : IDisposable
    {
        private readonly ArcfDirectory root = new("root");

        private bool isDisposed = false;

        public ArcfEncoder()
        {
            
        }

        public void AddFile(ArcfFile file, string directoryPath)
        {
            if (isDisposed)
                throw new Exception("[ArcfEncoder] ArcfEncoder has been disposed!");

            if (file.Stream == null || !file.Stream.CanRead)
                throw new Exception("[ArcfEncoder] Encoder ArcfFile streams must not be null or non-readable!");

            //add relative root directory if necessary
            ArcfDirectory encoderDirectory = AddRelativeDirectory(directoryPath);
            encoderDirectory.Files.Add(file);

#if DEBUG
            Console.WriteLine($"[ArcfEncoder] Added ArcfFile: {file.Name} to: {directoryPath}");
#endif

        }

        public void AddFile(ArcfFile file)
        {
            root.Files.Add(file);

#if DEBUG
            Console.WriteLine($"[ArcfEncoder] Added ArcfFile: {file.Name} to root");
#endif

        }

        private ArcfDirectory? SearchForDirectory(string directoryPath)
        {
            string[] directoryNames = directoryPath.Split('\\');

            if (directoryNames.Length == 0)
                throw new Exception("No directory names in path");

            ArcfDirectory result = root;

            //Search for the directories with the same name in the same order
            for (int i = 0; i < directoryNames.Length; i++)
            {
                //Search for the directory with the same name

                bool found = false;
                foreach (ArcfDirectory directory in result.Subdirectories)
                {
                    if (directory.Name == directoryNames[i])
                    {
                        result = directory;
                        found = true;
                        break;
                    }
                }

                //Directory not found, doesn't exist
                if (!found)
                    return null;
            }
            
            return result;
        }

        public ArcfDirectory AddRelativeDirectory(string directoryPath)
        {
            if (isDisposed)
                throw new Exception("[ArcfEncoder] ArcfEncoder has been disposed!");

            string[] directoryNames = directoryPath.Split('\\');

            if (directoryNames.Length == 0)
                throw new Exception("No directory names in path");

            ArcfDirectory directory = root;

            //Search for the directories with the same name in the same order
            for (int i = 0; i < directoryNames.Length; i++)
            {
                //Search for the directory with the same name

                bool found = false;
                foreach (ArcfDirectory subdirectory in directory.Subdirectories)
                {
                    if (subdirectory.Name == directoryNames[i])
                    {
                        directory = subdirectory;
                        found = true;
                        break;
                    }
                }

                //Directory not found, create
                if (!found)
                {

                    ArcfDirectory newDirectory = new(directoryNames[i]);
                    directory.Subdirectories.Add(newDirectory);

#if DEBUG
                    Console.WriteLine($"[ArcfEncoder] Added ArcfDirectory: {newDirectory.Name} to: {directory.Name}");
#endif

                    directory = newDirectory;
                }
            }

            return directory;
        }

        /// <summary>
        /// Writes the output to the stream
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public void Encode(Stream outputStream)
        {
            if (isDisposed)
                throw new Exception("[ArcfEncoder] ArcfEncoder has been disposed!");

            if (!outputStream.CanWrite)
                throw new Exception("[ArcfEncoder] Can not write to output stream!");

            //For testing purposes
#if DEBUG
            Console.WriteLine("[ArcfWriter] Opening output stream writer - Test");
#endif

            StreamWriter sw = new(outputStream);

#if DEBUG
            Console.WriteLine("[ArcfEncoder] Writing to stream...");
#endif

            sw.WriteLine("ARCF_EXT");
            sw.WriteLine("V1_TEST");

            void WriteDirectory(ArcfDirectory directory, int level = 0)
            {

                string tab = "";

#if DEBUG
                for (int i = 0; i < level; i++)
                    tab += "    ";
#endif

                sw.WriteLine(tab + directory.Name);

                Console.WriteLine($"[ArcfEncoder] Writing ArcfDirectory: {directory.Name} subdirectories");

                //Here begin the subdirectories of this directory
                sw.WriteLine(tab + "\\sub");

                foreach (ArcfDirectory subdirectory in directory.Subdirectories)
                {
                    WriteDirectory(subdirectory, level + 1);
                }


                Console.WriteLine($"[ArcfEncoder] Writing ArcfDirectory: {directory.Name} files");

                //Here begin the files of this directory
                sw.WriteLine(tab + "\\files");

                foreach (ArcfFile file in directory.Files)
                {
#if DEBUG
                    Console.WriteLine($"[ArcfEncoder] Writing ArcfFile: {file.Name}");
#endif

                    sw.WriteLine(tab + file.Name);

                    //TODO: file data
                }

                //Directory finished
                sw.WriteLine(tab + "\\parent");
            }

            WriteDirectory(root, 0);

            Console.WriteLine($"[ArcfEncoder] Finished writing ArcfFiles & ArcfDirectories");

            //File finished
            sw.WriteLine("\\end");

            sw.Flush();

            Console.WriteLine($"[ArcfEncoder] Finished writing to stream");
        }

        public void DisposeArcfDirectory(ArcfDirectory directory)
        {
            foreach (ArcfFile file in directory.Files)
                file.Stream?.Dispose();

            foreach (ArcfDirectory subdirectory in directory.Subdirectories)
                DisposeArcfDirectory(subdirectory);
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                DisposeArcfDirectory(root);

                isDisposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
