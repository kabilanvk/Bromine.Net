using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Helpers;
using Bromine.SeleniumWrapper.Models;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
    public class AddAuthHeaderTask : BrowserTask
    {
        public override Task<TaskResult> Process(TaskInfo task)
        {
            if (!(task is AddAuthHeaderTaskInfo)) throw new ArgumentException($"Expected AddHeaderTaskInfo but passed {task.GetType().Name}");

            var result = new TaskResult(false);
            var taskInfo = (AddAuthHeaderTaskInfo)task;
            CurrentBrowser.HeaderSetup.Authorization = taskInfo.HeaderValue;
            CurrentBrowser.SetHeader();
            return result.Success();
        }
    }
}
