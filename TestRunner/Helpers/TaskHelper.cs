using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Interfaces;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper;
using Bromine.TestRunner.Models;
using log4net;

namespace Bromine.TestRunner.Helpers
{
  internal class TaskHelper
  {
    private readonly ILog _log = LogHelper.GetLogger();
    private readonly TestExecutionContext _execContext;

    public TaskHelper(TestExecutionContext execContext)
    {
      _execContext = execContext;
    }

    public async Task<TaskResult> ExecuteTask(TestInfo testInfo, TaskInfo taskInfo)
    {
      var testContext = _execContext.Wrapper.GetTestContext(taskInfo, _execContext);
      var taskResult = new TaskResult(false);
      try
      {
        if (testContext.IsCacheSupported)
        {
          _execContext.Storage.Cache.ReplaceCacheKeys(taskInfo);
        }
        _log.Debug($"{taskInfo.TaskName} Comment: {taskInfo.Comment}");
        taskResult = await testContext.Execute(taskInfo, _execContext.Storage);
        if (!taskResult.Success && testInfo.Browser != null)
          _log.Info($"[Browser:{testInfo.Browser.Id}] Current Url={testInfo.Browser.Driver.Url}");
      }
      catch (BrowserException ex)
      {
        CloseCurrentBrowser(testInfo, testContext, ex, taskResult);
      }
      catch (OpenQA.Selenium.WebDriverException ex)
      {
        CloseCurrentBrowser(testInfo, testContext, ex, taskResult);
      }
      catch (Exception ex)
      {
        _log.Error($"{ex.Message}\nStackTrace: {ex.StackTrace}");
      }
      if (testContext.IsSeleniumUsed && Constants.AppConfig.IsScreenshotOn)
      {
        //Comment out this block. It's randomly giving exception, and making some
        //test to fail - e.g. Purchase.NMS.GoogleInAppFailure.RetryWithValidPSN.json
        //CaptureScreenshot(taskInfo);
      }
      return taskResult;
    }

    //private void CaptureScreenshot(TaskInfo currenTaskInfo)
    //{
    //  //Lookup selenium wrapper
    //  var iTestContext = _execContext.Wrapper.GetTestContextFromCache(Constants.WrapperName.SeleniumWrapper);

    //  //Create Common Task to take snapshot
    //  if (iTestContext == null) return;
    //  var task = new TaskInfo
    //  {
    //    Parent = currenTaskInfo.Parent,
    //    TaskName = Constants.Task.TakeScreenShot,
    //    Action = Constants.Task.TakeScreenShot
    //  };
    //  iTestContext.Execute(task, _execContext.Storage);
    //}

    private void CloseCurrentBrowser(TestInfo testInfo, ITestContext context, Exception ex, TaskResult result)
    {
      _log.Error($"[Browser:{testInfo.Browser?.Id}] {ex.Message} so, closing the browser.{(_log.IsDebugEnabled ? $"\nStackTrace: {ex.StackTrace}" : string.Empty)}");
      ((SeleniumTestContext)context).Dispose(testInfo.Browser);
      testInfo.Browser = null;
      result.IsError = true;
    }
  }
}
