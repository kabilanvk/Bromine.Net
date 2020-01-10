using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Models;

namespace Bromine.SeleniumWrapper.Tasks.ConsoleLog
{
  public class MatchLogTask : BrowserTask
  {
    private const string ResourceIdentifier = "res://";

    public override Task<TaskResult> Process(TaskInfo task)
    {
      if (!(task is LogTaskInfo)) throw new ArgumentException($"Expected LogTaskInfo but passed {task.GetType().Name}");

      var result = new TaskResult(false);
      var taskInfo = (LogTaskInfo)task;
      if (taskInfo.Ordered != null && taskInfo.Ordered.Count > 0)
      {
        var orderedStrings = GetNormalizedValues(taskInfo.Ordered);
        result.Success = ConsoleLogHelper.PerformMatch(orderedStrings) == !task.IsNegated;
        if (!result.Success) return result.Result();
      }
      if (taskInfo.UnOrdered != null && taskInfo.UnOrdered.Count > 0)
      {
        var unOrderedStrings = GetNormalizedValues(taskInfo.UnOrdered);
        result.Success = ConsoleLogHelper.PerformMatchWithoutOrder(unOrderedStrings) == !task.IsNegated;
        return result.Result();
      }
      if (taskInfo.Regex == null || taskInfo.Regex.Count <= 0) return result.Result();
      result.Success = ConsoleLogHelper.PerformRegexMatch(taskInfo.Regex, task.IsNegated);
      return result.Result();
    }

    private static IEnumerable<string> GetNormalizedValues(IEnumerable<string> matchStrings)
    {
      return matchStrings.Select(str => str.StartsWith(ResourceIdentifier)
          ? TestCore.Resources[str.Replace(ResourceIdentifier, string.Empty)]
          : str)
        .ToList();
    }
  }
}
