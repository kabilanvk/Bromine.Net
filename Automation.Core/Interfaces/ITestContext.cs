using System.Threading.Tasks;
using Bromine.Automation.Core.Models;

namespace Bromine.Automation.Core.Interfaces
{
  public interface ITestContext
  {
    Task<TaskResult> Execute(TaskInfo taskInfo, TestStorage storage);
    void Dispose();
    bool IsCacheSupported { get; }
    bool IsSeleniumUsed { get; }
  }
}
