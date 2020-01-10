using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;

namespace Bromine.GenericWrapper.Tasks
{
  internal class ClearCacheValueTask : BaseTask
  {
    public override Task<TaskResult> Process(TaskInfo task)
    {
      if (!(task is CacheTaskInfo)) throw new ArgumentException($"Expected CacheTaskInfo but passed {task.GetType().Name}");

      var taskInfo = (CacheTaskInfo)task;
      foreach (var data in taskInfo.CacheData)
      {
        var cacheKey = data.CacheKey;
        Storage.Cache.Remove(cacheKey);
        Logger.Info($"Cleared cache for '{cacheKey}'...");
      }

      return new TaskResult(true).Result();
    }
  }

}
