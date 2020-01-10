using System.Threading.Tasks;
using Bromine.Automation.Core.Models;

namespace Bromine.Automation.Core.Interfaces
{
    public interface ITask
    {
        Task<TaskResult> Process(TaskInfo task);
    }
}
