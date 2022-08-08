// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace Stellinator.Interfaces
{
    /// <summary>
    /// Represents a file to process.
    /// </summary>
    public class AstroFile
    {
        /// <summary>
        /// Source path.
        /// </summary>
        private string sourcePath;

        /// <summary>
        /// Gets or sets the observation value.
        /// </summary>
        public string Observation { get; set; }

        /// <summary>
        /// Gets or sets the date of the observation.
        /// </summary>
        public DateTime ObservationDate { get; set; }

        /// <summary>
        /// Gets or sets the observation sequence.
        /// </summary>
        public string ObservationSequence { get; set; }

        /// <summary>
        /// Gets or sets the capture information.
        /// </summary>
        public string Capture { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file was parsed from the new format.
        /// </summary>
        public bool IsNewFormat { get; set; }

        /// <summary>
        /// Gets or sets the source path of the file.
        /// </summary>
        public string SourcePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(sourcePath))
                {
                    string date;

                    try
                    {
                        date = ObservationDate.ToString("yyyy-MM-dd");
                    }
                    catch
                    {
                        date = "nodate";
                    }

                    return $"{Observation}|{date}|{Capture}|{FileName}.{FileExtension}"
                        .Replace('|', Path.DirectorySeparatorChar);
                }

                return sourcePath;
            }

            set => sourcePath = value;
        }

        /// <summary>
        /// Gets or sets the target path of the file.
        /// </summary>
        public string TargetPath { get; set; }

        /// <summary>
        /// Gets or sets the local (not full path) name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the renamed file name.
        /// </summary>
        public string NewFileName { get; set; }

        /// <summary>
        /// Gets the filename to match for processed (not raw) files
        /// to find a correlated FITS.
        /// </summary>
        public string FileNameMatch => IsProcessed ?
            FileName :
            FileName.EndsWith('r') ?
                $"{FileName.TrimEnd('r')}-output" :
                $"{FileName}-output";

        /// <summary>
        /// Gets or sets the file extension without the period.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file is a processed jpeg or tiff.
        /// </summary>
        public bool IsProcessed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file is a raw FITS file.
        /// </summary>
        public bool IsRaw { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file was rejected.
        /// </summary>
        public bool Rejected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file is a valid Stellina artifact.
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        /// Gets the list of parent directories to refresh timestamps.
        /// </summary>
        public List<string> Directories { get; } = new List<string>();

        /// <summary>
        /// Equality implementation.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>A value indicating whether the object is an <see cref="AstroFile"/> with the same source path.</returns>
        public override bool Equals(object obj) =>
            obj is AstroFile astroFile &&
            astroFile.SourcePath == SourcePath;

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code of the source path.</returns>
        public override int GetHashCode() =>
            SourcePath.GetHashCode();

        /// <summary>
        /// Gets the string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            var key = $"{Observation}:{ObservationDate}:{ObservationSequence}:{Capture}=>{FileName}.{FileExtension}";
            var status = Valid ? "VALID " : "INVALID ";
            var accepted = Rejected ? "REJECTED " : "ACCEPTED ";
            var baseStr = $"{status}\t{accepted}\t{key}";
            if (string.IsNullOrWhiteSpace(NewFileName))
            {
                return baseStr;
            }

            if (string.IsNullOrWhiteSpace(TargetPath))
            {
                return $"{baseStr} => {NewFileName}";
            }

            return $"{baseStr} => {TargetPath}";
        }
    }
}
