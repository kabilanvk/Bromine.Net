using System.IO;
using System.Threading;
using log4net.Appender;
using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bromine.TestRunner.Extensions
{
    /// <inheritdoc />
    /// <summary>
    /// Test-context appender, writing all logging events to the given <c>TestContext</c>.
    /// </summary>
    public sealed class TestContextAppender : AppenderSkeleton
    {
        #region IAppender members

        protected override void Append(LoggingEvent loggingEvent)
        {
            using var writer = new StringWriter();
            Layout.Format(writer, loggingEvent);
            TestContext?.Value?.WriteLine(writer.ToString());
        }

        protected override bool RequiresLayout => true;

        #endregion

        public AsyncLocal<TestContext> TestContext { get; set; } = new AsyncLocal<TestContext>();
    }
}
