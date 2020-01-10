using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Interfaces;
using Bromine.Automation.Core.Models;
using log4net;

namespace Bromine.Automation.Core.Common
{
  public class BaseTask : ITask
  {
    protected readonly ILog Logger = LogHelper.GetLogger();

    public TestStorage Storage { get; set; }

    public virtual Task<TaskResult> Process(TaskInfo task)
    {
      Logger.Info("No task to handle. Just returning success.");
      return new TaskResult(true).Result();
    }
  }
}
