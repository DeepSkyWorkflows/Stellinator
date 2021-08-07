using System;
using System.Collections.Generic;
using System.IO;
using Stellinator.Interfaces;
using Xunit;

namespace StellinatorTests
{
    public class AstroFileTests : IClassFixture<WriterSinkFixture>
    {
        public AstroFileTests(WriterSinkFixture _)
        {

        }

        private const string SOURCE_PATH = @"E:\at\at\joes";
        private const string NONEWFILENAME = "nonewfilename";
        private const string NOTARGETPATH = "notargetpath";
        private const string TARGETPATH = "targetpath";

        [Fact]
        public void FileWatch_Returns_File_With_Output()
        {
            // arrange
            var astroFile = new AstroFile
            {
                FileExtension = "fits",
                FileName = "img-001",
            };

            // act
            var actual = astroFile.FileNameMatch;

            // assert
            Assert.Equal($"{astroFile.FileName}-output", actual);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SourcePath_Uses_Observation_Info_When_No_Path_Set(
            bool sourcePathSet)
        {
            var date = DateTime.UtcNow;
            var dateStr = date.ToString("yyyy-MM-dd");

            // arrange
            var astro = new AstroFile
            {
                Observation = "moon",
                ObservationDate = date,
                Capture = "capture1",
                FileName = "damoon",
                FileExtension = "jpeg",
            };

            string expected;

            if (sourcePathSet)
            {
                expected = @"E:\damoon.jpeg";
                astro.SourcePath = expected; 
            }
            else
            {
                expected = $"moon|{dateStr}|capture1|damoon.jpeg"
                    .Replace('|', Path.DirectorySeparatorChar);
            }

            // assert
            Assert.Equal(expected, astro.SourcePath);
        }

        public static IEnumerable<object[]> EqualityMatrix()
        {
            var sourcePath = SOURCE_PATH;
            var match = new AstroFile { SourcePath = sourcePath };
            var otherMatch = new AstroFile { SourcePath = sourcePath };
            var notMatch = new AstroFile { SourcePath = @"F:\ranks\is\better" };
            var alsoNotMatch = new { SourcePath = sourcePath };
            yield return new object[]
            {
                match, true
            };

            yield return new object[]
            {
                otherMatch, true
            };

            yield return new object[]
            {
                notMatch, false
            };

            yield return new object[]
            {
                alsoNotMatch, false
            };
        }

        [Theory]
        [MemberData(nameof(EqualityMatrix))]
        public void Equality_Matches_Type_And_SourcePath(
            object obj, bool equal)
        {
            // arrange
            var source = new AstroFile { SourcePath = SOURCE_PATH };

            // act
            var areEqual = source.Equals(obj);

            // assert
            Assert.Equal(equal, areEqual);
        }

        [Theory]
        [InlineData(@"E:\")]
        [InlineData(@"E:\arth\day")]
        [InlineData(@"B:\et\you\dont\have\a\b\drive")]
        public void HashCode_Is_HashCode_Of_SourcePath(string path)
        {
            // arrange
            var expected = path.GetHashCode();
            var astro = new AstroFile { SourcePath = path };

            // act
            var actual = astro.GetHashCode();

            // assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(NONEWFILENAME)]
        [InlineData(NOTARGETPATH)]
        [InlineData(TARGETPATH)]
        public void ToString_Does_A_Lot_Of_Stuff(string scenario)
        {
            // arrange
            var astro = new AstroFile
            {
                Capture = nameof(AstroFile.Capture),
                FileExtension = "jpeg",
                FileName = "img001",
                IsProcessed = true,
                IsRaw = false,
                Observation = "arrakis",
                ObservationDate = DateTime.UtcNow,
                ObservationSequence = "22",
                Rejected = false,
                SourcePath = SOURCE_PATH,
                Valid = true,
            };

            if (scenario != NONEWFILENAME)
            {
                astro.NewFileName = "newleaf";
            }

            if (scenario == TARGETPATH)
            {
                astro.TargetPath = @"T:\urn\over\a\newleaf.jpeg";
            }

            // act
            var str = astro.ToString();

            // assert
            Assert.Contains(astro.FileName, str);
            Assert.Contains(astro.FileExtension, str);

            if (scenario != NONEWFILENAME)
            {
                Assert.Contains(astro.NewFileName, str);
            }

            if (scenario == TARGETPATH)
            {
                Assert.Contains(astro.TargetPath, str);
            }
        }
    }
}
