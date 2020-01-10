using System;
using System.Threading;
using System.Threading.Tasks;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Models;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
    public class NavigateTask : BrowserTask
    {
        public override Task<TaskResult> Process(TaskInfo task)
        {
            if (!(task is NavigateTaskInfo)) throw new ArgumentException($"Expected NavigateTaskInfo but passed {task.GetType().Name}");

            var result = new TaskResult(false);
            var taskInfo = (NavigateTaskInfo)task;
            var url = Utilities.ConfigureUrl(taskInfo.Url);
            if (string.IsNullOrEmpty(url)) return result.Failed();
            Info($"Launching url '{url}'");
            Storage.Summary.LogData[SummaryFields.LaunchUrl] = url;
            
            CurrentBrowser.Driver.Navigate().GoToUrl(url);
            // Adding a wait after redirect to resolve page lookup and logs not found issue
            Thread.Sleep(2000);

            return result.Success();
        }
    }
}
