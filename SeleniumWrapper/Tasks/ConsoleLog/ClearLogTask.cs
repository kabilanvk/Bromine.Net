using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;

namespace Bromine.SeleniumWrapper.Tasks.ConsoleLog
{
    public class ClearLogTask : BrowserTask
    {
        public override Task<TaskResult> Process(TaskInfo task)
        {
            // Reset and ready for new log
            ConsoleLogHelper.Clear();
            return new TaskResult(true).Result();
        }
    }
}
