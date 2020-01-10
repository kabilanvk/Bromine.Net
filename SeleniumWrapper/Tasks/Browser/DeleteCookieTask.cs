using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Models;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
  public class DeleteCookieTask : BrowserTask
  {
    public override Task<TaskResult> Process(TaskInfo task)
    {
      if (!(task is CookieTaskInfo)) throw new ArgumentException($"Expected CookieTaskInfo but passed {task.GetType().Name}");

      var result = new TaskResult(false);
      var taskInfo = (CookieTaskInfo)task;
      if (taskInfo.Cookies == null || taskInfo.Cookies.Count == 0)
      {
        CurrentBrowser.Driver.Manage().Cookies.DeleteAllCookies();
        return result.Success();
      }
      foreach (var cookie in taskInfo.Cookies)
      {
        if (!string.IsNullOrEmpty(cookie.Key))
        {
          CurrentBrowser.Driver.Manage().Cookies.DeleteCookieNamed(cookie.Key);
        }
      }

      return result.Success();
    }
  }
}
