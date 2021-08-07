// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;

namespace Stellinator.Workflow
{
    /// <summary>
    /// Tracks progress.
    /// </summary>
    public class ProgressCounter
    {
        private readonly long intervalTicks;
        private readonly long startTime;
        private readonly int totalCount;
        private long lastCheck;
        private int currentCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressCounter"/> class.
        /// </summary>
        /// <param name="totalCount">The total count.</param>
        /// <param name="interval">The interval between readouts.</param>
        public ProgressCounter(int totalCount, TimeSpan interval)
        {
            intervalTicks = interval.Ticks;
            lastCheck = DateTime.UtcNow.Ticks;
            startTime = lastCheck;
            currentCount = 0;
            this.totalCount = totalCount;
        }

        /// <summary>
        /// Gets the percentage complete as an integer.
        /// </summary>
        private int Pct => (int)(currentCount * 100.0) / totalCount;

        /// <summary>
        /// Increment the counter.
        /// </summary>
        public void Increment()
        {
            var now = DateTime.UtcNow.Ticks;

            var show = currentCount == 0 ||
                currentCount == (totalCount - 1) ||
                (now - lastCheck) > intervalTicks;

            currentCount++;

            if (show)
            {
                PrintProgress();
                lastCheck = now;
            }
        }

        /// <summary>
        /// Prints a progress bar.
        /// </summary>
        private void PrintProgress()
        {
            var asterisk = new string('*', Pct);
            var dash = new string('-', 100 - Pct);
            WorkflowWriter.WriteLine($"|{asterisk}{Pct}%{dash}|");

            var now = DateTime.UtcNow.Ticks;
            var span = now - startTime;
            var perFile = (double)span / currentCount;
            var remaining = (totalCount - currentCount) * perFile;
            WorkflowWriter.WriteLine($"{Remaining(TimeSpan.FromTicks((long)remaining))}");
        }

        private string Remaining(TimeSpan time)
        {
            var totalSeconds = time.TotalSeconds;
            if (totalSeconds < 60)
            {
                return $"{totalSeconds} seconds remaining.";
            }

            var minutes = totalSeconds / 60;
            if (minutes < 60)
            {
                var remainder = minutes - Math.Floor(minutes);
                var seconds = (int)(remainder * 60);
                return $"{(int)minutes} minute(s) {seconds} second(s) remaining.";
            }

            var hours = minutes / 60;
            var hoursRemainder = hours - Math.Floor(hours);
            var minutesLeft = (int)(hoursRemainder * 60);
            return $"{(int)hours} hour(s) {minutesLeft} minute(s) remaining.";
        }
    }
}
