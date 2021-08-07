using Stellinator.Workflow;

namespace StellinatorTests
{
    /// <summary>
    /// This fixture sets the writer to a noop to avoid dead-end
    /// console writes during tests.
    /// </summary>
    public class WriterSinkFixture
    {
        public WriterSinkFixture()
        {
            WorkflowWriter.SetCustomWriter(msg => { });
        }
    }
}
