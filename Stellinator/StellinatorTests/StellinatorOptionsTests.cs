using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Stellinator.Configuration;
using Xunit;

namespace StellinatorTests
{
    public class StellinatorOptionsTests : IClassFixture<WriterSinkFixture>
    {
        public StellinatorOptionsTests(WriterSinkFixture _)
        {

        }

        public static IEnumerable<object[]> ConfigurationMatrix()
        {
            var recurse = new[] { true, false };
            var quietMode = new[] { true, false };
            var scanOnly = new[] { true, false };
            var includeScope = new[] { true, false };

            var ignoreFlags = new[]
            {
                IgnoreFlags.Nothing,
                IgnoreFlags.AllButLast,
                IgnoreFlags.Rejection,
                IgnoreFlags.Tiff,
                IgnoreFlags.Jpeg,
                IgnoreFlags.Rejected
            };

            var groupStrategies = new[]
            {
                GroupStrategy.Observation,
                GroupStrategy.Date,
                GroupStrategy.Capture
            };

            var targetFilenameStragies = new[]
            {
                TargetFilenameStrategy.New,
                TargetFilenameStrategy.Original,
                TargetFilenameStrategy.Ticks,
                TargetFilenameStrategy.TicksHex
            };

            (int rIdx, int qIdx, int sIdx, int scIdx, int ifIdx, int gIdx, int tfIdx)
            = (0, 0, 0, 0, 0, 0, 0);

            while (tfIdx < targetFilenameStragies.Length)
            {
                yield return new object[]
                {
                    quietMode[qIdx],
                    recurse[rIdx],
                    scanOnly[sIdx],
                    includeScope[scIdx],
                    ignoreFlags[ifIdx],
                    groupStrategies[gIdx],
                    targetFilenameStragies[tfIdx]
                };

                qIdx++;
                if (qIdx == quietMode.Length)
                {
                    qIdx = 0;
                    rIdx++;
                    if (rIdx == recurse.Length)
                    {
                        rIdx = 0;
                        sIdx++;
                        if (sIdx == scanOnly.Length)
                        {
                            sIdx = 0;
                            scIdx++;
                            if (scIdx == includeScope.Length)
                            {
                                scIdx = 0;
                                ifIdx++;
                                if (ifIdx == ignoreFlags.Length)
                                {
                                    ifIdx = 0;
                                    gIdx++;
                                    if (gIdx == groupStrategies.Length)
                                    {
                                        gIdx = 0;
                                        tfIdx++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(ConfigurationMatrix))]
        public void Constructor_Sets_Values(
            bool quietMode,
            bool recurseOption,
            bool scanOption,
            bool includeScope,
            IgnoreFlags ignoreFlags,
            GroupStrategy groupStrategy,
            TargetFilenameStrategy targetFilenameStrategy)
        {
            var src = Guid.NewGuid().ToString();
            var tgt = Guid.NewGuid().ToString();

            var newName = targetFilenameStrategy == TargetFilenameStrategy.New
                ? "NewImage" : null;

            // arrange and act
            var options = new Options(
                recurseOption,
                quietMode,
                scanOption,
                includeScope,
                ignoreFlags,
                groupStrategy,
                targetFilenameStrategy,
                newName,
                src,
                tgt);

            // assert
            Assert.Equal(recurseOption, options.DirectoryOnly);
            Assert.Equal(quietMode, options.QuietMode);
            Assert.Equal(scanOption, options.ScanOnly);
            Assert.Equal(includeScope, options.IncludeScope);
            Assert.Equal(ignoreFlags, options.Ignore);
            Assert.Equal(groupStrategy, options.GroupStrategy);
            Assert.Equal(targetFilenameStrategy, options.TargetFilenameStrategy);
            Assert.Equal(newName, options.NewFilename);
            Assert.Equal(src, options.SourceDirectory);
            Assert.Equal(tgt, options.TargetDirectory);
        }

        public static IEnumerable<object[]> ToStringMatrix() =>
            ConfigurationMatrix().Where(
                item => (bool)item[0] == false);

        [Theory]
        [MemberData(nameof(ToStringMatrix))]
        public void ToString_Shows_Values(
            bool quietOption,
            bool recurseOption,
            bool scanOption,
            bool includeScope,
            IgnoreFlags ignoreFlags,
            GroupStrategy groupStrategy,
            TargetFilenameStrategy targetFilenameStrategy)
        {
            if (quietOption)
            {
                return;
            }

            // arrange
            var src = Guid.NewGuid().ToString();
            var tgt = Guid.NewGuid().ToString();

            var newName = targetFilenameStrategy == TargetFilenameStrategy.New
                ? "NewImage" : null;

            // arrange and act
            var options = new Options(
                recurseOption,
                false,
                scanOption,
                includeScope,
                ignoreFlags,
                groupStrategy,
                targetFilenameStrategy,
                newName,
                src,
                tgt);

            // act
            var text = options.ToString();

            var lines = text.Split(
                new[]
                {
                    '\r',
                    '\n'
                });

            static (string name, string value) Resolver<T>(
                Options options,
                Expression<Func<Options, T>> expr)
            {
                var value = options.ParseOption(expr)[40..];
                var lambda = expr as LambdaExpression;
                var member = lambda.Body as MemberExpression;
                return (member.Member.Name, value);
            }

            foreach (var (optionName, value) in new[]
            {
                Resolver(options, opt => opt.DirectoryOnly),
                Resolver(options, opt => opt.GroupStrategy),
                Resolver(options, opt => opt.Ignore),
                Resolver(options, opt => opt.ScanOnly),
                Resolver(options, opt => opt.TargetFilenameStrategy),
                Resolver(options, opt => opt.NewFilename)
            })
            {
                if (optionName != nameof(Options.NewFilename) ||
                    options.TargetFilenameStrategy == TargetFilenameStrategy.New)
                {
                    Assert.Contains(lines, l =>
                        l.Contains(optionName) && l.Contains(value));
                }
            }
        }

        [Theory]
        [InlineData(nameof(Options.DirectoryOnly), "False")]
        [InlineData(nameof(Options.GroupStrategy), "Capture")]
        [InlineData(nameof(Options.Ignore), "Rejected, Jpeg")]
        [InlineData(nameof(Options.NewFilename), "new")]
        [InlineData(nameof(Options.QuietMode), "False")]
        [InlineData(nameof(Options.ScanOnly), "False")]
        [InlineData(nameof(Options.SourceDirectory), @"e:\")]
        [InlineData(nameof(Options.TargetDirectory), @"f:\")]
        [InlineData(nameof(Options.TargetFilenameStrategy), "New")]
        public void ParseOptions_Parses_To_Value(
            string name, string value)
        {
            // arrange
            var options = new Options(
                    false,
                    false,
                    false,
                    false,
                    IgnoreFlags.Jpeg | IgnoreFlags.Rejected,
                    GroupStrategy.Capture,
                    TargetFilenameStrategy.New,
                    "new",
                    @"e:\",
                    @"f:\");

            // act
            var actual = name switch
            {
                nameof(Options.DirectoryOnly) => options.ParseOption(opt => opt.DirectoryOnly),
                nameof(Options.GroupStrategy) => options.ParseOption(opt => opt.GroupStrategy),
                nameof(Options.Ignore) => options.ParseOption(opt => opt.Ignore),
                nameof(Options.NewFilename) => options.ParseOption(opt => opt.NewFilename),
                nameof(Options.QuietMode) => options.ParseOption(opt => opt.QuietMode),
                nameof(Options.ScanOnly) => options.ParseOption(opt => opt.ScanOnly),
                nameof(Options.SourceDirectory) => options.ParseOption(opt => opt.SourceDirectory),
                nameof(Options.TargetDirectory) => options.ParseOption(opt => opt.TargetDirectory),
                _ => options.ParseOption(opt => opt.TargetFilenameStrategy)
            };

            // assert
            Assert.StartsWith(name, actual);
            Assert.EndsWith(value, actual);
        }

        [Fact]
        public void Constructor_Throws_On_Flags_Combined_With_Nothing()
        {
            // arrange, act, and assert
            Assert.Throws<InvalidOperationException>(
                () => new Options(
                    false,
                    false,
                    false,
                    false,
                    IgnoreFlags.Nothing | IgnoreFlags.Rejected,
                    GroupStrategy.Capture,
                    TargetFilenameStrategy.New,
                    "new",
                    @"e:\",
                    @"f:\"));;
        }

        public static IEnumerable<object[]> FlagMatrix()
        {
            yield return new object[] { IgnoreFlags.Nothing };

            var combinations = new[]
            {
                IgnoreFlags.Jpeg,
                IgnoreFlags.Rejected,
                IgnoreFlags.Rejection,
                IgnoreFlags.Tiff
            };

            var maxMask = 0x01 << combinations.Length;
            for (var idx = 0; idx <= maxMask; idx++)
            {
                var flag = IgnoreFlags.Nothing;
                var pos = 0;
                while (pos < combinations.Length)
                {
                    var mask = 1 << pos;
                    if ((mask & idx) > 0)
                    {
                        flag = flag == IgnoreFlags.Nothing ?
                            combinations[pos] :
                            flag | combinations[pos];
                    }
                    pos++;
                }

                if (flag != IgnoreFlags.Nothing)
                {
                    yield return new object[] { flag };
                }
            }
        }

        [Theory]
        [MemberData(nameof(FlagMatrix))]
        public void HasFlag_Handles_Flags(IgnoreFlags flags)
        {
            // arrange
            var options = new Options(
                    false,
                    false,
                    false,
                    false,
                    flags,
                    GroupStrategy.Capture,
                    TargetFilenameStrategy.New,
                    "new",
                    @"e:\",
                    @"f:\");

            var flagsToTest = Enum.GetValues<IgnoreFlags>();
            foreach (var flag in flagsToTest)
            {
                var expected = ((int)flag & (int)flags) > 0;

                // act
                var actual = options.HasIgnoreFlag(flag);

                // assert
                Assert.Equal(expected, actual);
            }
        }

        [Theory]
        [MemberData(nameof(FlagMatrix))]
        public void Constructor_Supports_Multiple_IgnoreFlags(IgnoreFlags flags)
        {
            // arrange and act
            var options = new Options(
                    false,
                    false,
                    false,
                    false,
                    flags,
                    GroupStrategy.Capture,
                    TargetFilenameStrategy.New,
                    "new",
                    @"e:\",
                    @"f:\");

            // assert
            Assert.Equal(flags, options.Ignore);
        }

        [Theory]
        [MemberData(nameof(FlagMatrix))]
        public void ToString_Shows_All_Flags(IgnoreFlags flags)
        {
            // arrange
            // arrange and act
            var options = new Options(
                    false,
                    false,
                    false,
                    false,
                    flags,
                    GroupStrategy.Capture,
                    TargetFilenameStrategy.New,
                    "new",
                    @"e:\",
                    @"f:\");

            // act
            var str = options.ToString();

            // assert
            foreach (IgnoreFlags flag in Enum.GetValues<IgnoreFlags>())
            {
                if (((int)flags & (int)flag) > 0)
                {
                    Assert.Contains(flag.ToString(), str);
                }
            }
        }
    }
}
