// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;

namespace Stellinator.Configuration
{
    /// <summary>
    /// Flags to build the target directory.
    /// </summary>
    [Flags]
    public enum TargetFilenameStrategy
    {
        /// <summary>
        /// Keep the original.
        /// </summary>
        Original,

        /// <summary>
        /// Replace with a provided name.
        /// </summary>
        New,

        /// <summary>
        /// Use numeric ticks.
        /// </summary>
        Ticks,

        /// <summary>
        /// Use hexadecimal ticks.
        /// </summary>
        TicksHex,
    }
}
