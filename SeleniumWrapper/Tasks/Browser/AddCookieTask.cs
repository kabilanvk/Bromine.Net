using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Models;
using OpenQA.Selenium;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
  public class AddCookieTask : BrowserTask
  {
    public override Task<TaskResult> Process(TaskInfo task)
    {
      if (!(task is CookieTaskInfo)) throw new ArgumentException($"Expected CookieTaskInfo but passed {task.GetType().Name}");

      var result = new TaskResult(false);
      var taskInfo = (CookieTaskInfo)task;
      foreach (var cookie in taskInfo.Cookies)
      {
        var cookieName = cookie.Key;
        var cookieValue = cookie.Value;
        if (string.IsNullOrEmpty(cookieName) || string.IsNullOrEmpty(cookieValue)) continue;
        var browserCookie = new Cookie(cookieName, cookieValue);
        CurrentBrowser.Driver.Manage().Cookies.AddCookie(browserCookie);
      }

      return result.Success();
    }
  }
}
