// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

namespace Stellinator.Configuration
{
    /// <summary>
    /// Strategy for grouping files.
    /// </summary>
    public enum GroupStrategy
    {
        /// <summary>
        /// Group by observed object.
        /// </summary>
        Observation,

        /// <summary>
        /// Group by observed object and date.
        /// </summary>
        Date,

        /// <summary>
        /// Group by observed object, date, and capture.
        /// </summary>
        Capture,
    }
}
