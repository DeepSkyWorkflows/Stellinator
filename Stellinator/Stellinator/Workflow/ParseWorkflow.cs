// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Stellinator.Configuration;
using Stellinator.Interfaces;

namespace Stellinator.Workflow
{
    /// <summary>
    /// Parses the initial set of files.
    /// </summary>
    public class ParseWorkflow : IWorkflow
    {
        private readonly IFileSystem fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseWorkflow"/> class.
        /// </summary>
        /// <param name="fileSystem">The <see cref="IFileSystem"/> implementation.</param>
        public ParseWorkflow(IFileSystem fileSystem) =>
            this.fileSystem = fileSystem;

        /// <summary>
        /// Processes the files.
        /// </summary>
        /// <param name="options">The file options.</param>
        /// <param name="files">The files (empty).</param>
        /// <returns>The processed files.</returns>
        public AstroFile[] Process(Options options, AstroFile[] files)
        {
            WorkflowWriter.WriteLine($"Processing input files starting at '{options.SourceDirectory}'.");

            if (!fileSystem.DirectoryExists(options.SourceDirectory))
            {
                throw new InvalidOperationException("Unable to find source directory '{options.SourceDirectory}'.");
            }

            var directoriesToParse = options.DirectoryOnly == false ?
                RecurseSubdirectories(options.SourceDirectory, options.QuietMode) :
                new[] { options.SourceDirectory };

            WorkflowWriter.WriteLine("Processing files...");

            var allFiles = directoriesToParse.SelectMany(d => fileSystem.GetFiles(d))
                .Where(f => f.Contains("stellina"))
                .Select(f => ParseFile(f)).ToArray();

            var stellina = allFiles.Count(a => a.Valid);
            var processed = allFiles.Count(a => a.IsProcessed);

            WorkflowWriter.WriteLine($"Parsed {allFiles.Length} files: {stellina} are Stellina files, {processed} are processed (jpg, tif)" +
                $" files.");

            return allFiles;
        }

        /// <summary>
        /// Parses the file information.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The parsed <see cref="AstroFile"/>.</returns>
        private static AstroFile ParseFile(string path)
        {
            var processedExtensions = new[] { "jpg", "jpeg", "tif", "tiff" };

            path = path.ToLowerInvariant();

            var parts = path.Split(new[]
            {
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar,
            });

            var result = new AstroFile
            {
                SourcePath = path,
                FileName = Path.GetFileNameWithoutExtension(parts[^1]),
                FileExtension = Path.GetExtension(parts[^1])[1..],
            };

            if (parts.Length < 4 || !parts[^2].Contains("capture") || !parts[^3].Contains("observation"))
            {
                result.Valid = false;
                return result;
            }

            result.Capture = parts[^2];
            result.IsProcessed = (
                processedExtensions.Contains(result.FileExtension) &&
                    result.FileName.Contains("-output")) ||
                    result.FileExtension.StartsWith("tif");
            result.IsRaw = result.FileExtension == "fits";
            result.Valid = result.FileExtension == "fits" || processedExtensions.Contains(result.FileExtension);

            var observationParts = parts[^3].Split('_');
            result.ObservationDate = DateTime.ParseExact(observationParts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var sequenceAndName = observationParts[1].Split("-observation-");
            result.ObservationSequence = sequenceAndName[0];
            result.Observation = sequenceAndName[1];

            return result;
        }

        /// <summary>
        /// Builds an array of recursed subdirectories.
        /// </summary>
        /// <param name="rootPath">The path to begin with.</param>
        /// <param name="quietMode">A value indicating whether to suppress verbosity.</param>
        /// <returns>All subdirectories.</returns>
        private string[] RecurseSubdirectories(string rootPath, bool quietMode)
        {
            var directoryStack = new Stack<string>();
            var directoryList = new List<string>();

            directoryStack.Push(rootPath);

            do
            {
                var dir = directoryStack.Pop();

                if (!quietMode)
                {
                    WorkflowWriter.WriteLine($"Visiting directory '{dir}'.");
                }

                directoryList.Add(dir);

                foreach (var subDir in fileSystem.GetSubdirectories(dir))
                {
                    if (fileSystem.DirectoryExists(subDir))
                    {
                        directoryStack.Push(subDir);
                    }
                }
            }
            while (directoryStack.Count > 0);

            WorkflowWriter.WriteLine($"Parsed {directoryList.Count} items.");
            return directoryList.ToArray();
        }
    }
}
