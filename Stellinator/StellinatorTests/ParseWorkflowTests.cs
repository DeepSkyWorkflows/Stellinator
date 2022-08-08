using System;
using Stellinator.Configuration;
using Stellinator.Interfaces;
using Stellinator.Workflow;
using Xunit;

namespace StellinatorTests
{
    public class ParseWorkflowTests : IClassFixture<WriterSinkFixture>
    {
        private readonly IFileSystem fileSystem;
        private readonly IWorkflow parseWorkflow;

        public ParseWorkflowTests(WriterSinkFixture _)
        {
            fileSystem = new TestFileSystem();
            parseWorkflow = new ParseWorkflow(fileSystem);
        }

        [Fact]
        public void Throws_Invalid_Operation_When_Source_Doesnt_Exist()
        {
            var options = GetOptions(@"C:\TGT\");
            Assert.Throws<InvalidOperationException>(
                () => parseWorkflow.Process(options, Array.Empty<AstroFile>()));
        }

        [Fact]
        public void Does_Nothing_If_Stellina_Directory_Not_Found()
        {
            var options = GetOptions(@"C:\SRC\");
            var files = parseWorkflow.Process(options, Array.Empty<AstroFile>());
        }

        private static Options GetOptions(string srcDir)
        {
            return new Options(
                   false,
                   false,
                   false,
                   false,
                   IgnoreFlags.Nothing,
                   GroupStrategy.Date,
                   TargetFilenameStrategy.Ticks,
                   null,
                   srcDir,
                   @"E:\TGT");
        }

    }
}
