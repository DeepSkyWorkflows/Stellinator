// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Linq;
using Stellinator.Configuration;
using Stellinator.Interfaces;

namespace Stellinator.Workflow
{
    /// <summary>
    /// Filters based on <see cref="IgnoreFlags"/>.
    /// </summary>
    public class FilterWorkflow : IWorkflow
    {
        /// <summary>
        /// Process the files.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="files">The files to process.</param>
        /// <returns>The filtered files.</returns>
        public AstroFile[] Process(Options options, AstroFile[] files)
        {
            int counter = 0;
            int filtered = 0;
            int rejected = 0;
            int accepted = 0;

            bool IsJpegFilter(AstroFile file) =>
                file.FileExtension.StartsWith("jp");

            bool JpegFilter(AstroFile file) =>
                options.HasIgnoreFlag(IgnoreFlags.Jpeg) &&
                IsJpegFilter(file);

            bool IsTiffFilter(AstroFile file) =>
                file.FileExtension.StartsWith("tif");

            bool TiffFilter(AstroFile file) =>
                options.HasIgnoreFlag(IgnoreFlags.Tiff) &&
                IsTiffFilter(file);

            var (lastJpeg, lastTiff) = (string.Empty, string.Empty);

            bool ignoreAllButLast = options.HasIgnoreFlag(IgnoreFlags.AllButLast);

            foreach (var file in files)
            {
                counter++;
                if (file.IsProcessed)
                {
                    if (JpegFilter(file) || TiffFilter(file))
                    {
                        file.Valid = false;
                        filtered++;
                        continue;
                    }

                    if (ignoreAllButLast)
                    {
                        if (IsJpegFilter(file))
                        {
                            lastJpeg = file.SourcePath;
                        }
                        else if (IsTiffFilter(file))
                        {
                            lastTiff = file.SourcePath;
                        }
                    }
                }

                if (options.HasIgnoreFlag(IgnoreFlags.Rejection) == false
                    && file.IsRaw && !file.IsNewFormat)
                {
                    var match = files.Any(f => f.FileName == file.FileNameMatch);
                    file.Rejected = !match;
                    if (file.Rejected)
                    {
                        rejected++;
                        if (options.HasIgnoreFlag(IgnoreFlags.Rejected))
                        {
                            filtered++;
                            file.Valid = false;
                        }
                    }
                    else
                    {
                        accepted++;
                    }
                }
                else
                {
                    accepted++;
                }
            }

            if (ignoreAllButLast)
            {
                var filesToIgnore = files.Where(
                    f => f.IsProcessed &&
                     ((IsJpegFilter(f) && f.SourcePath != lastJpeg) ||
                        (IsTiffFilter(f) && f.SourcePath != lastTiff)));
                foreach (var file in filesToIgnore)
                {
                    file.Valid = false;
                }
            }

            WorkflowWriter.WriteLine($"Processed {counter} files: {accepted} accepted, {rejected} rejected, {filtered} filtered.");

            return files;
        }
    }
}
