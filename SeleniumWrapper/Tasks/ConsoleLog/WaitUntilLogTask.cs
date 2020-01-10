using System;
using System.Threading;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Models;

namespace Bromine.SeleniumWrapper.Tasks.ConsoleLog
{
  public class WaitUntilLogTask : BrowserTask
  {
    private const int TimeoutSeconds = 20;
    public override Task<TaskResult> Process(TaskInfo task)
    {
      if (!(task is LogTaskInfo)) throw new ArgumentException($"Expected LogTaskInfo but passed {task.GetType().Name}");

      var result = new TaskResult(false);
      var taskInfo = (LogTaskInfo)task;
      var matchLog = taskInfo.Log;
      var waitTimeout = taskInfo.Timeout;
      var noOfOccurrence = taskInfo.Occurrence;
      if (string.IsNullOrEmpty(matchLog)) return result.Result();
      // Wait time in seconds, default is 20
      waitTimeout = waitTimeout == 0 ? TimeoutSeconds : waitTimeout;
      // No of occurrences to wait, default is 1
      noOfOccurrence = noOfOccurrence == 0 ? 1 : noOfOccurrence;
      Info($"Waiting for log '{matchLog}' atleast {noOfOccurrence} occurrence(s)");
      var success = false;
      var loop = 0;
      while (loop < waitTimeout)
      {
        var logs = ConsoleLogHelper.GetConsoleLog(true, false);
        var index = logs.IndexOf(matchLog, StringComparison.OrdinalIgnoreCase);
        if (index >= 0)
        {
          var found = 1;
          var currentPos = index;
          while (found < noOfOccurrence)
          {
            index = logs.IndexOf(matchLog, currentPos + 1, StringComparison.OrdinalIgnoreCase);
            // No match
            if (index < 0) break;
            currentPos = index;
            found++;
          }
          if (found == noOfOccurrence)
          {
            success = true;
            break;
          }
        }
        loop += 2;
        Thread.Sleep(2000);
      }
      Info($"{(success ? "Success!" : "Failed!")} wait ended after '{loop}' secs.");
      result.Success = success;
      return result.Result();
    }
  }
}
