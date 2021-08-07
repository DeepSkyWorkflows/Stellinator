// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.IO;
using System.Linq;
using Stellinator.Interfaces;

namespace Stellinator.FileSystem
{
    /// <summary>
    /// Implementation of file system.
    /// </summary>
    public class FileSystemHandler : IFileSystem
    {
        /// <summary>
        /// Copies the file to the target, adding a sequence if the file
        /// already exists.
        /// </summary>
        /// <param name="src">The source file.</param>
        /// <param name="tgt">The target file.</param>
        public void CopyFile(string src, string tgt)
        {
            var seq = 0;

            var target = tgt;
            var extension = Path.GetExtension(tgt);
            do
            {
                if (FileExists(target))
                {
                    seq++;
                    var seqStr = string.Format("{0:00}", seq);
                    var targetDir = Path.GetDirectoryName(tgt);
                    target =
                        Path.Combine(
                            targetDir,
                            $"{Path.GetFileNameWithoutExtension(tgt)} ({seqStr}){extension}");
                }
                else
                {
                    break;
                }
            }
            while (seq < 100);

            if (FileExists(target))
            {
                throw new InvalidOperationException($"Unable to copy file '{src}' to target '{tgt}'");
            }

            File.Copy(src, target, false);
        }

        /// <summary>
        /// Creates a directory.
        /// </summary>
        /// <param name="path">The path to the directory to create.</param>
        public void CreateDirectory(string path) =>
            Directory.CreateDirectory(path);

        /// <summary>
        /// Checks if the directory exists.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>A value indicating whether the directory exists.</returns>
        public bool DirectoryExists(string path) => Directory.Exists(path);

        /// <summary>
        /// Checks if the file exists.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>A value indicating whether the file exists.</returns>
        public bool FileExists(string path) => File.Exists(path);

        /// <summary>
        /// Gets the list of files under the path.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <returns>The file list.</returns>
        public string[] GetFiles(string path) =>
            Directory.EnumerateFiles(path).ToArray();

        /// <summary>
        /// Gets the subdirectories under the current one.
        /// </summary>
        /// <param name="path">The path to the parent.</param>
        /// <returns>The subdirectories.</returns>
        public string[] GetSubdirectories(string path) =>
            Directory.EnumerateDirectories(path).ToArray();

        /// <summary>
        /// Updates the last written to date of the target path.
        /// </summary>
        /// <param name="path">The path to touch.</param>
        public void Touch(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.SetLastWriteTimeUtc(path, DateTime.UtcNow);
            }
        }
    }
}
