// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Linq;
using Stellinator.Configuration;
using Stellinator.Interfaces;

namespace Stellinator.Workflow
{
    /// <summary>
    /// Processing of files.
    /// </summary>
    public class ProcessWorkflow : IWorkflow
    {
        private readonly IWorkflow filterWorkflow;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessWorkflow"/> class.
        /// </summary>
        /// <param name="filterWorkflow">The filter workflow.</param>
        public ProcessWorkflow(IWorkflow filterWorkflow) =>
            this.filterWorkflow = filterWorkflow;

        /// <summary>
        /// Process the files.
        /// </summary>
        /// <param name="options">The <see cref="Options"/>.</param>
        /// <param name="files">The files.</param>
        /// <returns>A mysterious object comprised solely of <see cref="AstroFile"/>.</returns>
        public AstroFile[] Process(Options options, AstroFile[] files)
        {
            var groups = files.Where(f => f.Valid)
                .Select(f => f.Observation).Distinct();

            Action<string> log = options.QuietMode ?
                msg => { } : WorkflowWriter.WriteLine;

            foreach (var group in groups)
            {
                log($"Observation: {group}");

                var dates = files.Where(f => f.Observation == group)
                    .Select(f => f.ObservationDate).Distinct();

                foreach (var date in dates)
                {
                    log($"\tObservation Date: {date}");

                    var captures = files.Where(f => f.Observation == group
                    && f.ObservationDate == date)
                        .Select(f => f.Capture).Distinct();

                    foreach (var capture in captures)
                    {
                        log($"\t\tCapture: {capture}");
                        var filesToProcess = files.Where(f => f.Observation == group
                            && f.ObservationDate == date
                            && f.Capture == capture)
                            .OrderBy(f => f.FileExtension)
                            .ThenBy(f => f.FileName)
                            .ToArray();

                        filesToProcess = filterWorkflow.Process(options, filesToProcess);
                    }
                }
            }

            return files.Where(f => f.Valid).ToArray();
        }
    }
}
