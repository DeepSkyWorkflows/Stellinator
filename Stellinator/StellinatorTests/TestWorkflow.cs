using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stellinator.Configuration;
using Stellinator.Interfaces;

namespace StellinatorTests
{
    public class TestWorkflow : IWorkflow
    {
        private readonly int seq;

        public TestWorkflow(int seq) => this.seq = seq;

        public AstroFile[] Process(Options options, AstroFile[] files)
        {
            files ??= Array.Empty<AstroFile>();

            foreach (var file in files)
            {
                file.Valid = false;
            }

            var newFiles = new[]
            {
                new AstroFile()
                {
                    FileName = seq.ToString(),
                    FileExtension = "test",
                    Valid = true
                }
            };

            return newFiles.Union(files).ToArray();
        }
    }
}
