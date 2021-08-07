// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using Stellinator.Configuration;
using Stellinator.Interfaces;

namespace Stellinator.Workflow
{
    /// <summary>
    /// Validate the source directory.
    /// </summary>
    public class ValidateWorkflow : IWorkflow
    {
        private readonly IFileSystem fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateWorkflow"/> class.
        /// </summary>
        /// <param name="fileSystem">The <see cref="IFileSystem"/> implementation.</param>
        public ValidateWorkflow(IFileSystem fileSystem) =>
            this.fileSystem = fileSystem;

        /// <summary>
        /// Process the validation.
        /// </summary>
        /// <param name="options">The <see cref="Options"/>.</param>
        /// <param name="files">The files to process.</param>
        /// <returns>A value indicating whether validation passed.</returns>
        public AstroFile[] Process(Options options, AstroFile[] files)
        {
            if (!fileSystem.DirectoryExists(options.SourceDirectory))
            {
                throw new InvalidOperationException($"Invalid source directory: '{options.SourceDirectory}'.");
            }

            return files;
        }
    }
}
