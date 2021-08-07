// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Stellinator.Configuration;
using Stellinator.Interfaces;

namespace Stellinator.Workflow
{
    /// <summary>
    /// Copies the files.
    /// </summary>
    public class CopyWorkflow : IWorkflow
    {
        private readonly IFileSystem fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyWorkflow"/> class.
        /// </summary>
        /// <param name="fileSystem">The <see cref="IFileSystem"/> implementation.</param>
        public CopyWorkflow(IFileSystem fileSystem) =>
            this.fileSystem = fileSystem;

        /// <summary>
        /// Process (copy) the files.
        /// </summary>
        /// <param name="options">The <see cref="Options"/>.</param>
        /// <param name="files">The files to process.</param>
        /// <returns>The processed file list.</returns>
        public AstroFile[] Process(Options options, AstroFile[] files)
        {
            if (options.ScanOnly && options.QuietMode)
            {
                return files;
            }

            string directory = string.Empty;

            var directoriesToUpdate = new Dictionary<string, DateTime>();

            int dirCount = 0;
            int fileCount = 0;
            int totalCount = files.Where(f => f.Valid).Count();

            var progress = new ProgressCounter(totalCount, TimeSpan.FromSeconds(5));

            foreach (var file in
                files.Where(f => f.Valid).OrderBy(f => f.TargetPath))
            {
                foreach (var dirName in file.Directories)
                {
                    if (directoriesToUpdate.ContainsKey(dirName) == false)
                    {
                        directoriesToUpdate.Add(dirName, DateTime.UtcNow);
                    }
                }

                if (options.ScanOnly)
                {
                    WorkflowWriter.WriteLine($"{file.SourcePath} => {file.TargetPath}");
                    fileCount++;
                    progress.Increment();
                    continue;
                }

                var dir = Path.GetDirectoryName(file.TargetPath);
                if (dir != directory)
                {
                    directory = dir;
                    if (!fileSystem.DirectoryExists(directory))
                    {
                        fileSystem.CreateDirectory(directory);
                        if (options.QuietMode == false)
                        {
                            WorkflowWriter.WriteLine($"Created directory: {directory}");
                        }

                        dirCount++;
                    }
                }

                fileSystem.CopyFile(file.SourcePath, file.TargetPath);
                fileCount++;
                progress.Increment();
                if (options.QuietMode == false)
                {
                    WorkflowWriter.WriteLine($"Copied {file.SourcePath} => {file.TargetPath}");
                }
            }

            if (options.ScanOnly)
            {
                WorkflowWriter.WriteLine($"Processed {fileCount} files.");
            }
            else
            {
                WorkflowWriter.WriteLine($"Created {dirCount} directories and copied {fileCount} files.");
                foreach (var key in directoriesToUpdate.Keys)
                {
                    Directory.SetLastWriteTimeUtc(key, directoriesToUpdate[key]);
                }
            }

            return files;
        }
    }
}
