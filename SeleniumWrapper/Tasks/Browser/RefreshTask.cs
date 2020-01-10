using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
  public class RefreshTask : BrowserTask
  {
    public override Task<TaskResult> Process(TaskInfo task)
    {
      CurrentBrowser.Driver.Navigate().Refresh();
      return new TaskResult(true).Result();
    }
  }
}
