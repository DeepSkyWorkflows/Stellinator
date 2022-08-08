using System.Linq;
using Stellinator.Configuration;
using Stellinator.Interfaces;
using Stellinator.Workflow;
using Xunit;

namespace StellinatorTests
{
    public class FilterWorkflowTests : IClassFixture<WriterSinkFixture>
    {
        private readonly IWorkflow filterWorkflow;

        public FilterWorkflowTests(WriterSinkFixture _)
        {
            filterWorkflow = new FilterWorkflow();
        }

        private static AstroFile[] GetTestFiles(int iterations = 1)
        {
            var files = new[]
            {
                new AstroFile
                {
                    FileName = "avatar",
                    FileExtension = "tiff",
                    IsProcessed = true,
                    Valid = true,
                },

                new AstroFile
                {
                    FileName = "accepted-output",
                    FileExtension = "jpeg",
                    IsProcessed = true,
                    Valid = true
                },

                new AstroFile
                {
                    FileName = "moon",
                    FileExtension = "jpeg",
                    IsProcessed = false,
                    Valid = true
                },

                new AstroFile
                {
                    FileName = "accepted",
                    FileExtension = "fits",
                    IsRaw = true,
                    Valid = true,
                },

                new AstroFile
                {
                    FileName = "rejected",
                    FileExtension = "fits",
                    IsRaw = true,
                    Valid = true,
                }
            };

            var result = files.Select(f => f).ToArray();

            while (--iterations > 0)
            {
                var copy = files.Select(f => f);
                foreach (var file in copy)
                {
                    file.FileName = $"{file.FileName}-{iterations}";
                }
                result = result.Union(copy).ToArray();
            }

            return result;
        }

        private static Options GetOptions(IgnoreFlags ignoreFlags) =>
            new(
                false,
                false,
                false,
                false,
                ignoreFlags,
                GroupStrategy.Date,
                TargetFilenameStrategy.TicksHex,
                null,
                @"f:\",
                @"e:\");

        [Theory]
        [InlineData(IgnoreFlags.Jpeg)]
        [InlineData(IgnoreFlags.Tiff)]
        public void Process_Invalidates_Filtered_Files(IgnoreFlags flag)
        {
            // arrange
            var files = GetTestFiles();
            var options = GetOptions(flag); 
            var extension = flag.ToString().ToLower();

            // act
            var filteredFiles = filterWorkflow.Process(options, files);

            // assert
            Assert.Single(
                filteredFiles.Where(f => f.IsProcessed),
                f => f.FileExtension == extension && f.Valid == false);
        }

        [Fact]
        public void Process_Filters_AllButLast_When_Set()
        {
            // arrange
            var files = GetTestFiles(5);
            var options = GetOptions(IgnoreFlags.AllButLast);
            
            // act
            var filteredFiles = filterWorkflow.Process(options, files);

            // assert
            Assert.Single(
                filteredFiles.Where(f => f.IsProcessed),
                f => f.FileExtension == "jpeg" && f.Valid == true);

            Assert.Single(
                filteredFiles.Where(f => f.IsProcessed),
                f => f.FileExtension == "tiff" && f.Valid == true);
        }

        [Fact]
        public void Process_Flags_Rejected_Files_As_Accepted_When_IgnoreFlags_Rejection()
        {
            // arrange
            var files = GetTestFiles();
            var options = GetOptions(IgnoreFlags.Rejection);

            // act
            var processedFiles = filterWorkflow.Process(options, files);

            // assert
            Assert.Single(
                processedFiles,
                f => f.FileName == "rejected" && f.Rejected == false
                && f.Valid);
        }

        [Fact]
        public void Process_Sets_Rejected_Flags_When_IgnoreFlags_Rejection_Not_Set()
        {
            // arrange
            var files = GetTestFiles();
            var options = GetOptions(IgnoreFlags.Nothing);

            // act
            var processedFiles = filterWorkflow.Process(options, files);

            // assert
            Assert.Single(
                processedFiles,
                f => f.FileName == "rejected" && f.Rejected == true
                && f.Valid);
        }

        [Fact]
        public void Process_Invalidates_Rejected_Files_When_IgnoreFlags_Rejected_Set()
        {
            // arrange
            var files = GetTestFiles();
            var options = GetOptions(IgnoreFlags.Rejected);

            // act
            var processedFiles = filterWorkflow.Process(options, files);

            // assert
            Assert.Single(
                processedFiles,
                f => f.FileName == "rejected" && f.Rejected == true
                && f.Valid == false);
        }
    }
}
