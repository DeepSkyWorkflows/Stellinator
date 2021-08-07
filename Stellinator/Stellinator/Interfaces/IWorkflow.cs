// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using Stellinator.Configuration;

namespace Stellinator.Interfaces
{
    /// <summary>
    /// Main workflow.
    /// </summary>
    public interface IWorkflow
    {
        /// <summary>
        /// Process options and files.
        /// </summary>
        /// <param name="options">The <see cref="Options"/>.</param>
        /// <param name="files">The list of files.</param>
        /// <returns>The processed <see cref="AstroFile"/>.</returns>
        AstroFile[] Process(Options options, AstroFile[] files);
    }
}
