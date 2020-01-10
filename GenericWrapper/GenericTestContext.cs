using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Interfaces;
using Bromine.Automation.Core.Models;
using Bromine.GenericWrapper.Tasks;
using log4net;

namespace Bromine.GenericWrapper
{
    public class GenericTestContext : ITestContext
    {
        private readonly ILog _logger = LogHelper.GetLogger();

        //Added location 
        public void Dispose() { }

        public bool IsCacheSupported => true;

        public bool IsSeleniumUsed => false;

        public Task<TaskResult> Execute(TaskInfo taskInfo, TestStorage storage)
        {
            try
            {
                var task = CreateTask(taskInfo);
                task.Storage = storage;
                return task.Process(taskInfo);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        private static BaseTask CreateTask(TaskInfo taskInfo)
        {
            BaseTask task;
            switch (taskInfo.Action)
            {
                case ActionType.AddTemplate:
                    task = new AddTemplateTask();
                    break;
                case ActionType.CompareValue:
                    task = new CompareValueTask();
                    break;
                case ActionType.GetValueFromCache:
                    task = new GetValueFromCacheTask();
                    break;
                case ActionType.ClearCacheValue:
                    task = new ClearCacheValueTask();
                    break;
                default:
                    task = new BaseTask();
                    break;
            }
            return task;
        }
    }
}
