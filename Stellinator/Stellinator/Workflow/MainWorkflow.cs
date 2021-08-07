// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using Stellinator.Configuration;
using Stellinator.Interfaces;

namespace Stellinator.Workflow
{
    /// <summary>
    /// Main processing logic.
    /// </summary>
    public class MainWorkflow : IWorkflow
    {
        /// <summary>
        /// The list of workflows.
        /// </summary>
        private readonly IWorkflow[] workflows;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWorkflow"/> class.
        /// </summary>
        /// <param name="workflows">The list of workflows to run.</param>
        public MainWorkflow(
            params IWorkflow[] workflows) =>
            this.workflows = workflows;

        /// <summary>
        /// Process options.
        /// </summary>
        /// <param name="options">The <see cref="Options"/>.</param>
        /// <param name="files">The list of files.</param>
        /// <returns>A zero when all goes well.</returns>
        public AstroFile[] Process(Options options, AstroFile[] files)
        {
            var processedFiles = files;
            foreach (var workflow in workflows)
            {
                processedFiles = workflow.Process(options, processedFiles);
            }

            return processedFiles;
        }
    }
}
