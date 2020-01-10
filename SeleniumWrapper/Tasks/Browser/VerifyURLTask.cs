using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Helpers;
using Bromine.SeleniumWrapper.Models;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
    public class VerifyUrlTask : BrowserTask
    {
        public override Task<TaskResult> Process(TaskInfo task)
        {
            if (!(task is VerifyUrlTaskInfo)) throw new ArgumentException($"Expected VerifyUrlTaskInfo but passed {task.GetType().Name}");

            var result = new TaskResult(false);
            var taskInfo = (VerifyUrlTaskInfo)task;
            foreach (var url in taskInfo.Urls)
            {
                var finalUrl = Utilities.ConfigureUrl(url);
                //Removing http(s) from urls always, if needed we can introduce a parameter based control
                finalUrl = finalUrl.Replace("https", string.Empty).Replace("http", string.Empty);
                result.Success = task.IsNegated ? !CurrentBrowser.Driver.VerifyUrl(finalUrl) : CurrentBrowser.Driver.WaitForUrl(finalUrl);
                if (result.Success) continue;
                Info($"Failed! Looking for '{finalUrl}' in URL {CurrentBrowser.Driver.Url}");
                return result.Failed();
            }
            return result.Success();
        }
    }
}
