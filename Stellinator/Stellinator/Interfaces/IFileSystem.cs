// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using Stellinator.Configuration;

namespace Stellinator.Interfaces
{
    /// <summary>
    /// Interface to abstract file I/O.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Check if a directory exists.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <returns>A value indicating whether the directory exists.</returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// Check if a file exists.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>A value indicating whether the file exists.</returns>
        bool FileExists(string path);

        /// <summary>
        /// Gets the list of subdirectories under the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The list of subdirectories.</returns>
        string[] GetSubdirectories(string path);

        /// <summary>
        /// Gets the list of files under the directory.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <returns>The list of files.</returns>
        string[] GetFiles(string path);

        /// <summary>
        /// Creates a directory.
        /// </summary>
        /// <param name="path">The path to the directory to create.</param>
        void CreateDirectory(string path);

        /// <summary>
        /// Copies the file to the target, adding a sequence if the file
        /// already exists.
        /// </summary>
        /// <param name="src">The source file.</param>
        /// <param name="tgt">The target file.</param>
        void CopyFile(string src, string tgt);

        /// <summary>
        /// Updates the last written to date of the target path.
        /// </summary>
        /// <param name="path">The path to touch.</param>
        void Touch(string path);
    }
}
