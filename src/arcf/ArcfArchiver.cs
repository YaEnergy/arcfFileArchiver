﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arcf
{
    public class ArcfArchiver : IDisposable
    {
        public ArcfWriter ArcfWriter
        {
            get => _arcfWriter;
        }

        private readonly ArcfWriter _arcfWriter;

        private bool isDisposed = false;

        public ArcfArchiver(Stream stream)
        {
            _arcfWriter = new(stream);
        }

        public ArcfArchiver(ArcfWriter arcfWriter)
        {
            _arcfWriter = arcfWriter;
        }

        public static void Archive(ArcfWriter writer)
            => writer.WriteTo();

        public void Archive()
            => Archive(_arcfWriter);

        #region Adding files

        public void AddFile(FileInfo file, string toRelativePath)
        {
            if (isDisposed)
                throw new Exception("[ArcfArchiver] ArcfArchiver has been disposed!");

            if (!file.Exists)
                throw new FileNotFoundException("[ArcfArchiver] File does not exist!", file.FullName);

#if DEBUG
            Console.WriteLine("[ArcfArchiver] Adding file " + file.Name + " to: " + toRelativePath);
#endif

            FileStream fileStream = file.OpenRead();
            _arcfWriter.AddStream(fileStream, toRelativePath);
        }

        public void AddFile(string filePath, string toRelativePath)
        {
            AddFile(new FileInfo(filePath), toRelativePath);
        }

        public void AddTopLevelFile(FileInfo file)
        {
            AddFile(file, file.Name);
        }

        public void AddTopLevelFile(string filePath)
        {
            AddTopLevelFile(new FileInfo(filePath));
        }

        #endregion

        #region Adding directories

        public void AddDirectory(DirectoryInfo directory, string toRelativePath, bool recursive = true)
        {
            if (isDisposed)
                throw new Exception("[ArcfArchiver] ArcfArchiver has been disposed!");

            if (!directory.Exists)
                throw new DirectoryNotFoundException("[ArcfArchiver] Directory (" + directory.FullName + ") does not exist!");

            Console.WriteLine("[ArcfArchiver] Adding directory " + directory.Name + " to: " + toRelativePath);

            _arcfWriter.AddRelativeDirectoryPath(toRelativePath);

            //Add files
            foreach (FileInfo file in directory.GetFiles())
            {
                AddFile(file, Path.TrimEndingDirectorySeparator(toRelativePath) + @"\" + file.Name);
            }

            if (recursive)
            {
                //Add subdirectories
                foreach (DirectoryInfo subdirectory in directory.GetDirectories())
                {
                    AddDirectory(subdirectory, Path.TrimEndingDirectorySeparator(toRelativePath) + @"\" + subdirectory.Name);
                }
            }
        }

        public void AddDirectory(string directoryPath, string toRelativePath, bool recursive = true)
        {
            AddDirectory(new DirectoryInfo(directoryPath), toRelativePath, recursive);
        }

        public void AddTopLevelDirectory(DirectoryInfo directory, bool recursive = true)
        {
            AddDirectory(directory, directory.Name, recursive);
        }

        public void AddTopLevelDirectory(string directoryPath, bool recursive = true)
        {
            AddTopLevelDirectory(new DirectoryInfo(directoryPath), recursive);
        }

        #endregion

        public void Dispose()
        {
            if (!isDisposed)
                _arcfWriter.Dispose();

            isDisposed = true;

            GC.SuppressFinalize(this);
        }
    }
}