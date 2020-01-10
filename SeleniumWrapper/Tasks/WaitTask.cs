using System;
using System.Threading;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Models;

namespace Bromine.SeleniumWrapper.Tasks
{
  public class WaitTask : BrowserTask
  {
    public override Task<TaskResult> Process(TaskInfo task)
    {
      if (!(task is LogTaskInfo)) throw new ArgumentException($"Expected LogTaskInfo but passed {task.GetType().Name}");

      var result = new TaskResult(false);
      var taskInfo = (LogTaskInfo)task;
      Thread.Sleep(taskInfo.Timeout * 1000);
      return result.Success();
    }
  }
}
