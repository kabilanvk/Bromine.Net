using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Models;
using Bromine.TestRunner.Extensions;
using Bromine.TestRunner.Helpers;
using Bromine.TestRunner.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestResult = Bromine.Automation.Core.Models.TestResult;

namespace Bromine.TestRunner
{
    public static class TestRunner
    {
        #region Variable Declaration

        public static int TotalTests { get; set; }
        private static int _testsRunCounter;
        private static int _successCount;
        private static int _failureCount;
        private static TestContext _testContext;
        private static List<WrapperHelper> _wrapperHelpers;

        #endregion

        public static void Initialize(TestContext testContext)
        {
            _testContext = testContext;
            _wrapperHelpers = new List<WrapperHelper>();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        public static async Task Runner(TestInfo testInfo)
        {
            // Test log
            LogHelper.SetLogger(testInfo);
            ApplyTestContextLogging(testInfo.Context);
            var rerunCount = 1;
            var log = LogHelper.GetDefaultLogger();
            var execContext = new TestExecutionContext(new TestStorage(), new WrapperHelper(), _testContext);
            lock (_wrapperHelpers)
            {
                _wrapperHelpers.Add(execContext.Wrapper);
            }
            var testResult = new TestResult();
            try
            {
                log.Info($"***** Running {++_testsRunCounter} of {TotalTests} *****");
                testResult = await new TestHelper(execContext).ExecuteTest(testInfo);
                if (!testResult.Success)
                {
                    // Rerun test when failed
                    var retryCount = Constants.AppConfig.RetryCount;
                    if (retryCount == 0 && testResult.Retry) retryCount++;
                    while (rerunCount <= retryCount)
                    {
                        var testLog = LogHelper.GetLogger();
                        testLog.Info($"\n##### Retrying {testInfo.Name} for {++rerunCount} time  #####");
                        // Resetting cache & summary 
                        execContext.Storage = new TestStorage();
                        testInfo.Browser = null;
                        testResult = await new TestHelper(execContext).ExecuteTest(testInfo);
                        if (testResult.Success) break;
                    }
                }
                Assert.IsTrue(testResult.Success, testInfo.Name);
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error($"Fatal Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                Assert.Fail($"{testInfo.Name} failed!");
            }
            finally
            {
                ReportStatus(execContext, testResult);
                log.Info($"***** {testResult.Status} in {rerunCount}/{Constants.AppConfig.RetryCount + 1} attempt(s) " +
                           $"Status=P:{_successCount},F:{_failureCount} *****");
            }
        }

        public static void CleanUp()
        {
            // Disposing wrapper testcontext
            lock (_wrapperHelpers)
            {
                foreach (var wrapper in _wrapperHelpers)
                {
                    wrapper.ResetTestContext();
                }
                _wrapperHelpers.Clear();
                _wrapperHelpers = null;
            }
            GC.Collect();
        }

        private static void ApplyTestContextLogging(TestContext testContext)
        {
            var appender = (TestContextAppender)LogHelper.GetLogger().Logger.Repository.GetAppenders().First(a => a.Name.Equals("TestAppender"));
            appender.TestContext.Value = testContext;
        }

        private static void ReportStatus(TestExecutionContext context, TestResult result)
        {
            if (!result.Success)
            {
                context.Storage.Summary.LogSummary();
                _failureCount += 1;
                return;
            }
            _successCount += 1;
        }
    }
}