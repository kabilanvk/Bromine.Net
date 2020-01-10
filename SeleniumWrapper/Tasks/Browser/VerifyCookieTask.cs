using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Models;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
    public class VerifyCookieTask : BrowserTask
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

                var cookieResponse = CurrentBrowser.Driver.Manage().Cookies.GetCookieNamed(cookieName);
                if (cookieResponse == null)
                {
                    if (task.IsNegated) continue;
                    Info($"Failed! cookie '{cookieName}' doesn't exists.");
                    return result.Failed();
                }
                if ((!string.IsNullOrEmpty(cookieValue) && cookieResponse.Value.Equals(cookieValue)) != task.IsNegated) continue;
                Info($"Failed! cookie '{cookieName}:{cookieValue}' {(task.IsNegated ? "" : "doesn't")} matches.");
                return result.Failed();
            }

            return result.Success();
        }
    }
}
