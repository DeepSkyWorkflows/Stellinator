// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;

namespace Stellinator.Workflow
{
    /// <summary>
    /// Manages application-specific output. Default outputs to the
    /// console.
    /// </summary>
    public static class WorkflowWriter
    {
        /// <summary>
        /// The writer.
        /// </summary>
        private static Action<string> writer = Console.WriteLine;

        /// <summary>
        /// Sets a custom writer.
        /// </summary>
        /// <param name="writer">The custom writer.</param>
        public static void SetCustomWriter(Action<string> writer)
            => WorkflowWriter.writer = writer;

        /// <summary>
        /// Send a message to the writer.
        /// </summary>
        /// <param name="msg">The message to send.</param>
        public static void WriteLine(string msg) => WorkflowWriter.writer(msg);
    }
}
