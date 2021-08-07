using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Stellinator.Interfaces;

namespace StellinatorTests
{
    public class TestFileSystem : IFileSystem
    {
        public struct FileOrDirectory
        {
            public string path;
            public bool isDirectory;

            public override int GetHashCode() => path.GetHashCode();
        }

        private readonly IDictionary<string, HashSet<FileOrDirectory>> fileSystem =
            new Dictionary<string, HashSet<FileOrDirectory>>();

        public TestFileSystem()
        {
            CreateFile(@"C:\src\fits.fits");
            CreateDirectory(@"C:\stellina\");
            CreateDirectory(@"E:\TGT");
        }

        // C:\
        // C:\SRC
        // E:\
        // E:\TGT

        public void CopyFile(string src, string tgt) => CreateFile(tgt);

        public void CreateDirectory(string path)
        {
            var parts = path.Split(@"\");
            var lastDir = string.Empty;
            for (var idx = 0; idx < parts.Length; idx++)
            {
                var dir = idx == 0 ? $"{parts[0]}" :
                    string.Join(@"\", parts.Take(idx + 1));

                dir = NormalizeDirectory(dir);

                if (fileSystem.ContainsKey(dir) == false)
                {
                    fileSystem.Add(dir, new HashSet<FileOrDirectory>());
                    if (string.IsNullOrWhiteSpace(lastDir) == false)
                    {
                        fileSystem[lastDir].Add(
                            new FileOrDirectory
                            {
                                isDirectory = true,
                                path = dir,
                            });
                    }
                    lastDir = dir;
                }
            }
        }

        public void CreateFile(string path)
        {
            var dir = NormalizeDirectory(
                Path.GetDirectoryName(path));
            CreateDirectory(dir);
            var file = new FileOrDirectory
            {
                isDirectory = false,
                path = path.ToUpperInvariant(),
            };
            fileSystem[dir].Add(file);
        }

        public bool DirectoryExists(string path)
        {
            var dir = NormalizeDirectory(path);
            return fileSystem.ContainsKey(dir);
        }

        public bool FileExists(string path) =>
            fileSystem.SelectMany(f => f.Value.Select(v => v))
            .Where(fod => fod.isDirectory == false)
            .Any(fod => fod.path == path.ToUpperInvariant());

        public string[] GetFiles(string path)
        {
            var dir = NormalizeDirectory(path);
            if (fileSystem.ContainsKey(dir))
            {
                return fileSystem[dir].Where(f => f.isDirectory == false)
                    .Select(f => f.path).ToArray();
            }

            return Array.Empty<string>();
        }

        public string[] GetSubdirectories(string path)
        {
            var dir = NormalizeDirectory(path);
            if (fileSystem.ContainsKey(dir))
            {
                return fileSystem[dir].Where(f => f.isDirectory == true)
                    .Select(f => f.path).ToArray();
            }

            return Array.Empty<string>();
        }

        private static string NormalizeDirectory(string dir)
        {
            var directory = dir.ToUpperInvariant();
            if (directory.EndsWith(Path.DirectorySeparatorChar))
            {
                return directory;
            }
            return $"{directory}{Path.DirectorySeparatorChar}";
        }

        /// <summary>
        /// Mock touching the timestamp.
        /// </summary>
        /// <param name="path">The path.</param>
        public void Touch(string path) {}
    }
}
