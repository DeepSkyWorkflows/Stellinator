// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace Stellinator.Configuration
{
    /// <summary>
    /// Configuration for the running process.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class.
        /// </summary>
        public Options()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class.
        /// </summary>
        /// <param name="directoryOnly">The recurse subdirectories option.</param>
        /// <param name="quietMode">The quiet mode option.</param>
        /// <param name="scanOnly">The scan only option.</param>
        /// <param name="includeScope">The scope include option.</param>
        /// <param name="ignoreFlags">The ignore flags option.</param>
        /// <param name="groupStrategy">The grouping strategy option.</param>
        /// <param name="targetFilenameStrategy">The filename renaming strategy option.</param>
        /// <param name="newFilename">Name of the new file.</param>
        /// <param name="sourceDirectory">The source (Stellina USB drive) directory.</param>
        /// <param name="targetDirectory">The target directory.</param>
        public Options(
            bool directoryOnly,
            bool quietMode,
            bool scanOnly,
            bool includeScope,
            IgnoreFlags ignoreFlags,
            GroupStrategy groupStrategy,
            TargetFilenameStrategy targetFilenameStrategy,
            string newFilename,
            string sourceDirectory,
            string targetDirectory)
        {
            if (!string.IsNullOrWhiteSpace(newFilename) &&
                targetFilenameStrategy != TargetFilenameStrategy.New)
            {
                throw new InvalidOperationException("New filename option is only valid when target filename strategy is set to 'New'.");
            }

            if (targetFilenameStrategy == TargetFilenameStrategy.New &&
                string.IsNullOrWhiteSpace(newFilename))
            {
                throw new InvalidOperationException("You must specify a new filename using the '-n' or '--new-filename' option when target filename strategy is set to 'New'.");
            }

            if (((int)ignoreFlags & (int)IgnoreFlags.Nothing) > 0 &&
                ignoreFlags != IgnoreFlags.Nothing)
            {
                throw new InvalidOperationException("Cannot use other ignores with 'Nothing'.");
            }

            DirectoryOnly = directoryOnly;
            QuietMode = quietMode;
            ScanOnly = scanOnly;
            IncludeScope = includeScope;
            Ignore = ignoreFlags;
            GroupStrategy = groupStrategy;
            TargetFilenameStrategy = targetFilenameStrategy;
            NewFilename = newFilename;
            SourceDirectory = sourceDirectory;
            TargetDirectory = targetDirectory;
        }

        /// <summary>
        /// Gets the usage examples.
        /// </summary>
        [Usage(ApplicationAlias = "stellinator")]
        public static IEnumerable<Example> Examples
            => new List<Example>()
            {
                new Example(
                    "Copy images sorting rejects and accepted and grouped by date",
                    new Options(false, false, false, false, IgnoreFlags.Nothing, GroupStrategy.Date, TargetFilenameStrategy.TicksHex, null, @"F:", @"E:")),
                new Example(
                    "Copy images from a single directory (ignore subdirectories)",
                    new Options(true, false, false, false, IgnoreFlags.Nothing, GroupStrategy.Date, TargetFilenameStrategy.TicksHex, null, @"F:", @"E:")),
                new Example(
                    "Do a test run without writing any files using the ticks naming strategy",
                    new Options(false, false, true, false, IgnoreFlags.Nothing, GroupStrategy.Date, TargetFilenameStrategy.Ticks, null, @"F:", @"E:")),
                new Example(
                    "Copy images treating rejects as accepted",
                    new Options(false, false, false, false, IgnoreFlags.Rejection, GroupStrategy.Date, TargetFilenameStrategy.Ticks, null, @"F:", @"E:")),
                new Example(
                    "Ignore (don't copy) rejected images to the target folders",
                    new Options(false, false, false, false, IgnoreFlags.Rejected, GroupStrategy.Date, TargetFilenameStrategy.Ticks, null, @"F:", @"E:")),
                new Example(
                    "Copy images from a single directory (ignore subdirectories) and include telescope in the destination file name",
                    new Options(true, false, false, true, IgnoreFlags.Nothing, GroupStrategy.Date, TargetFilenameStrategy.TicksHex, null, @"F:", @"E:")),
            };

        /// <summary>
        /// Gets a value indicating whether or not to recurse subdirectories.
        /// </summary>
        [Option('d', "directory-only", Default = false, HelpText = "Directory only, do not recurse subdirectories.")]
        public bool DirectoryOnly { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not to provide verbose messages.
        /// </summary>
        [Option('q', "quiet-mode", Default = false, HelpText = "Quiet mode (don't report all file actions).")]
        public bool QuietMode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not to scan but not update.
        /// </summary>
        [Option('s', "scan-only", Default = false, HelpText = "Scan only (don't actually update).")]
        public bool ScanOnly { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not to include the telescope name in the target directory.
        /// </summary>
        [Option('c', "include-scope", Default = false, HelpText = "Include scope name in destination path.")]
        public bool IncludeScope { get; private set; }

        /// <summary>
        /// Gets a value indicating whether rejects should be included.
        /// </summary>
        [Option('i', "ignore", Default = IgnoreFlags.Nothing, HelpText = "Choose what to ignore. Can combine multiple (comma separated). Rejection flag will treat rejected as accepted. Rejected flag will ignore (not copy) rejected. AllButLast will only copy the last process (TIFF or JPEG) image.")]
        public IgnoreFlags Ignore { get; private set; }

        /// <summary>
        /// Gets the strategy for grouping images.
        /// </summary>
        [Option('g', "group-strategy", Default = GroupStrategy.Date, HelpText = "Choose the strategy to group images.")]
        public GroupStrategy GroupStrategy { get; private set; }

        /// <summary>
        /// Gets the strategy to name the new files.
        /// </summary>
        [Option('t', "target-filename-strategy", Default = TargetFilenameStrategy.TicksHex, HelpText = "Choose the strategy to rename target images.")]
        public TargetFilenameStrategy TargetFilenameStrategy { get; private set; }

        /// <summary>
        /// Gets the new filename used when the <see cref="TargetFilenameStrategy"/> is set to <see cref="TargetFilenameStrategy.New"/>.
        /// </summary>
        [Option('n', "new-filename", Default = null, HelpText = "Set the name of target files when using target filename strategy 'New'.")]
        public string NewFilename { get; private set; }

        /// <summary>
        /// Gets the path to the source directory.
        /// </summary>
        [Value(0, MetaName = nameof(SourceDirectory), Required = true, HelpText = "The path to the Stellina USB drive folder to scan.")]
        public string SourceDirectory { get; private set; }

        /// <summary>
        /// Gets the path to the target directory.
        /// </summary>
        [Value(1, MetaName = nameof(TargetDirectory), Required = true, HelpText = "The path to the root of the target directory.")]
        public string TargetDirectory { get; private set; }

        /// <summary>
        /// Prints the options configuration.
        /// </summary>
        /// <returns>The list of configured options.</returns>
        public override string ToString()
        {
            var location = Process.GetCurrentProcess().MainModule.FileName;
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(location);
            var heading = $"{fileVersionInfo.ProductName} {fileVersionInfo.ProductVersion}";
            var copyright = fileVersionInfo.LegalCopyright;
            var sb = new StringBuilder($"{heading}{Environment.NewLine}{copyright}{Environment.NewLine}");

            if (!QuietMode)
            {
                sb.AppendLine("Running with options:");
                sb.AppendLine(ParseOption(opt => opt.DirectoryOnly));
                sb.AppendLine(ParseOption(opt => opt.QuietMode));
                sb.AppendLine(ParseOption(opt => opt.ScanOnly));
                sb.AppendLine(ParseOption(opt => opt.Ignore));
                sb.AppendLine(ParseOption(opt => opt.GroupStrategy));
                sb.AppendLine(ParseOption(opt => opt.TargetFilenameStrategy));
                if (TargetFilenameStrategy == TargetFilenameStrategy.New)
                {
                    sb.AppendLine(ParseOption(opt => opt.NewFilename));
                }

                sb.AppendLine(ParseOption(opt => opt.SourceDirectory));
                sb.AppendLine(ParseOption(opt => opt.TargetDirectory));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determine whether a flag is set.
        /// </summary>
        /// <param name="flag">The flag to check.</param>
        /// <returns>A value indicating whether the flag is set.</returns>
        public bool HasIgnoreFlag(IgnoreFlags flag) =>
            ((int)flag & (int)Ignore) > 0;

        /// <summary>
        /// Parses the property name and value for an option.
        /// </summary>
        /// <typeparam name="T">The type of thet option.</typeparam>
        /// <param name="option">An expression that resolves to the option.</param>
        /// <returns>The text to display the option settings.
        /// </returns>
        public string ParseOption<T>(Expression<Func<Options, T>> option)
        {
            const int left = 40;
            var value = option.Compile()(this);
            var lambda = option as LambdaExpression;
            var memberExpression = lambda.Body as MemberExpression;
            var name = memberExpression.Member.Name;
            var diff = left - name.Length;
            var pad = diff > 0 ? new string(' ', diff) : string.Empty;
            if (value is IgnoreFlags ignoreFlags)
            {
                var setting = (int)ignoreFlags;
                var display =
                    string.Join(
                        ", ",
                        Enum.GetValues<IgnoreFlags>().Select(
                            ignore => ((int)ignore & setting) > 0 ?
                            ignore.ToString() : string.Empty)
                        .Where(fl => fl != string.Empty));
                return $"{name}:{pad}{display}";
            }

            return $"{name}:{pad}{value}";
        }
    }
}
