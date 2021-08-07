// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;

namespace Stellinator.Configuration
{
    /// <summary>
    /// Flags to ignore files for copy.
    /// </summary>
    [Flags]
    public enum IgnoreFlags
    {
        /// <summary>
        /// Default: don't ignore anything.
        /// </summary>
        Nothing = 0x01,

        /// <summary>
        /// Ignore rejection files. Process them as accepted.
        /// </summary>
        Rejection = 0x02,

        /// <summary>
        /// Ignore rejected files.
        /// </summary>
        Rejected = 0x04,

        /// <summary>
        /// Ignore jpg and jpeg files.
        /// </summary>
        Jpeg = 0x08,

        /// <summary>
        /// Ignore TIF and TIFF files.
        /// </summary>
        Tiff = 0x10,

        /// <summary>
        /// Ignore all processed (TIFF, jpeg) files but the last.
        /// </summary>
        AllButLast = 0x20,
    }
}
