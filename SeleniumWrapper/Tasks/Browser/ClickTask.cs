using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Helpers;
using Bromine.SeleniumWrapper.Models;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
  public class ClickTask : BrowserTask
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
        
        if (!CurrentBrowser.Driver.PerformClick(lookupType.LocateBy(lookupValue))) return result.Result();

        // Until we figureout why anchor click spans a new tab with href link
        //if (!CurrentBrowser.Driver.Url.Equals("about:blank", StringComparison.OrdinalIgnoreCase)) continue;
        //// Sometimes its too fast to close the browser throws angular exception
        //System.Threading.Thread.Sleep(1000);
        //CurrentBrowser.Driver.FindElement(By.TagName("body")).SendKeys(Keys.Control + "w");
        //System.Threading.Thread.Sleep(1000);
        //CurrentBrowser.Driver.SwitchTo().DefaultContent();
      }
      return result.Success();
    }
  }
}
