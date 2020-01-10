using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Models;
using OpenQA.Selenium.Support.Extensions;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
  public class ExecuteJavaScriptTask : BrowserTask
  {
    public override Task<TaskResult> Process(TaskInfo task)
    {
      if (!(task is ScriptTaskInfo)) throw new ArgumentException($"Expected ScriptTaskInfo but passed {task.GetType().Name}");

      var result = new TaskResult(false);
      var taskInfo = (ScriptTaskInfo)task;
      foreach (var expression in taskInfo.Expressions)
      {
        var returnVal = CurrentBrowser.Driver.ExecuteJavaScript<string>(expression.Expression);
        //Only cache the value if return is not empty
        if (string.IsNullOrEmpty(returnVal)) continue;
        if (string.IsNullOrEmpty(expression.CacheKey))
        {
          Storage.Cache.AddOrUpdate(Constants.JsReturnValue, returnVal);
        }
        else
        {
          Storage.Cache.Add(expression.CacheKey, returnVal);
        }
        Info($"Script evaluated and return value '{returnVal}' cached");
      }

      return result.Success();
    }
  }
}
