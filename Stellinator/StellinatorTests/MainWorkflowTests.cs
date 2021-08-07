using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stellinator.Configuration;
using Stellinator.Workflow;
using Xunit;

namespace StellinatorTests
{
    public class MainWorkflowTests
    {
        [Fact]
        public void MainWorkflow_Calls_Subworkflows_In_Order()
        {
            var options = new Options(
                false,
                false,
                false,
                IgnoreFlags.Nothing,
                GroupStrategy.Date,
                TargetFilenameStrategy.TicksHex,
                null,
                @"E:\",
                @"F:\");

            // arrange
            var workflows = new[]
            {
                new TestWorkflow(1),
                new TestWorkflow(2),
                new TestWorkflow(3),
            };
            var mainWorkflow = new MainWorkflow(workflows);

            // act
            var files = mainWorkflow.Process(options, null);

            // assert
            Assert.Equal(
                files.OrderByDescending(f => f.FileName).First().FileName,
                files.Single(f => f.Valid).FileName);
        }

    }
}
