using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Helpers;
using Bromine.SeleniumWrapper.Models;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
  public class VerifyUiTask : BrowserTask
  {
    public override Task<TaskResult> Process(TaskInfo task)
    {
      if (!(task is UiTaskInfo)) throw new ArgumentException($"Expected UiTaskInfo but passed {task.GetType().Name}");

      var result = new TaskResult(false);
      var taskInfo = (UiTaskInfo)task;
      foreach (var ele in taskInfo.Elements)
      {
        var lookupType = ele.Value;
        var lookupValue = ele.Key;
        switch (lookupType)
        {
          case LocateByType.Text:
            result.Success = task.IsNegated ? CurrentBrowser.Driver.IsElementFound(lookupValue) : CurrentBrowser.Driver.IsElementFoundAfterWait(lookupValue);
            break;
          case LocateByType.Title:
            result.Success = CurrentBrowser.Driver.Title.Equals(lookupValue) == !task.IsNegated;
            break;
          default:
            var elementLocation = lookupType.LocateBy(lookupValue);
            result.Success = task.IsNegated
              ? CurrentBrowser.Driver.IsElementFound(elementLocation)
              : CurrentBrowser.Driver.IsElementFoundAfterWait(elementLocation);
            break;
        }
        if (result.Success) continue;
        Info($"Failed! '{lookupType}:{lookupValue}' {(task.IsNegated ? "" : "not")} found.");
        break;
      }
      return result.Result();
    }
  }
}
