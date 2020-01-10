using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Models;
using Bromine.GenericWrapper.Models;

namespace Bromine.GenericWrapper.Tasks
{
  public class AddTemplateTask : BaseTask
  {
    public override Task<TaskResult> Process(TaskInfo task)
    {
      if (!(task is TemplateTaskInfo)) throw new ArgumentException($"Expected TemplateTaskInfo but passed {task.GetType().Name}");

      var result = new TaskResult(false);
      var taskInfo = (TemplateTaskInfo)task;
      foreach (var param in taskInfo.Parameters)
      {
        var url = Utilities.ConfigureUrl(param.Value);
        Storage.Cache.Add(param.Key, url);
      }

      return result.Success();
    }
  }
}
