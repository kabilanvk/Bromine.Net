using System.Threading.Tasks;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Interfaces;
using Bromine.Automation.Core.Models;
using Bromine.RestServiceWrapper.Tasks;

namespace Bromine.RestServiceWrapper
{
    public class RestServiceTestContext : ITestContext
    {
      public void Dispose() { }

        public bool IsCacheSupported => false;

        public bool IsSeleniumUsed => false;

        public Task<TaskResult> Execute(TaskInfo taskInfo, TestStorage storage)
        {
            var task = CreateTask(taskInfo);
            task.Storage = storage;
            return task.Process(taskInfo);
        }

        private static BaseTask CreateTask(TaskInfo taskInfo)
        {
            BaseTask task;
            switch (taskInfo.Action)
            {
                case ActionType.SilentRequest:
                    task = new SilentRequest();
                    break;
                default:
                    task = new BaseTask();
                    break;
            }
            return task;
        }
    }
}
