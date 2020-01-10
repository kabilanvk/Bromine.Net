using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Helpers;
using Bromine.SeleniumWrapper.Models;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
  public class FetchDataFromUiTask : BrowserTask
  {
    public override Task<TaskResult> Process(TaskInfo task)
    {
      if (!(task is ElementTaskInfo)) throw new ArgumentException($"Expected ElementTaskInfo but passed {task.GetType().Name}");

      var result = new TaskResult(false);
      var taskInfo = (ElementTaskInfo)task;
      foreach (var ele in taskInfo.Elements)
      {
        var lookupType = ele.Type;
        var lookupValue = ele.Expression;
        var attribute = ele.Attribute;
        var cacheParam = ele.CacheKey;
        lookupValue = Utilities.ReplaceStringOfMultipleVariable(lookupValue);
        var element = CurrentBrowser.Driver.GetElement(lookupType.LocateBy(lookupValue), false);
        if (element == null)
        {
          Info($"No element '{lookupType}:{lookupValue}' found.");
          return result.Failed();
        }
        var elementValue = !string.IsNullOrEmpty(attribute) ? element.GetAttribute(attribute) : element.Text;
        Storage.Cache.Add(cacheParam, elementValue);
        Info($"Element '{lookupType}:{lookupValue}' found  with value '{elementValue}' and cached");
      }

      return result.Success();
    }
  }
}