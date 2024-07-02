using System;
using System.IO;

namespace Example
{
    public class Lesson
    {
        #region Phần 7 - Stream và File

        private string _directoryTree = string.Empty;

        private void GetDirectoryAndFile()
        {
            var path = $"C:\\";
            var dir = new DirectoryInfo(path);

            //var directories = Directory.GetDirectories(path);
            var directories = dir.GetDirectories();
            foreach (var directory in directories)
            {
                Console.WriteLine($"{directory.LastWriteTime:dd/MM/yyyy HH:mm}\t<DIR>\t\t{directory.Name}");
            }

            var files = dir.GetFiles();
            foreach (var file in files)
            {
                Console.WriteLine($"{file.LastWriteTime:dd/MM/yyyy HH:mm}\t{file.Length,20:#,###} bytes\t\t{file.Name}");
            }
        }

        private void Copy()
        {
            var source = @"C:\{source}";
            var destination = @"C:\{destination}";

            var buffer = new byte[1024];
            using var inputStream = File.OpenRead(source);
            using var outputStream = File.OpenRead(destination);

            int n = inputStream.Read(buffer, 0, buffer.Length);
            while (n > 0)
            {
                Console.WriteLine(n.ToString());

                outputStream.Write(buffer, 0, n);

                n = inputStream.Read(buffer, 0, buffer.Length);
            }

            //inputStream.Close();
            //outputStream.Close();
        }

        private string GetDirectoryTree(string path)
        {
            _directoryTree = string.Empty;
            var dir = new DirectoryInfo(path);

            DirectoryTreeRecursion(dir, 0);

            return _directoryTree;
        }

        private void DirectoryTreeRecursion(DirectoryInfo dir, int level)
        {
            var directories = dir.GetDirectories();
            int tab = level;

            if (directories.Length == 0) { }
            else
            {
                foreach (var directory in directories)
                {
                    _directoryTree += string.Concat(new String('\t', tab), $"|-{directory.Name}\n");
                    DirectoryTreeRecursion(directory, tab + 1);
                }
            }

            var files = dir.GetFiles();
            foreach (var file in files)
            {
                _directoryTree += string.Concat(new String('\t', tab), $"|-{file.Name}\n");
            }
        }

        #endregion
    }
}
