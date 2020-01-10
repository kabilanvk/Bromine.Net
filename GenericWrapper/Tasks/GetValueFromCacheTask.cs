using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Models;

namespace Bromine.GenericWrapper.Tasks
{
    public class GetValueFromCacheTask : BaseTask
    {
        public override Task<TaskResult> Process(TaskInfo task)
        {
            if (!(task is CacheTaskInfo)) throw new ArgumentException($"Expected CacheTaskInfo but passed {task.GetType().Name}");

            var taskResult = new TaskResult(false);
            var taskInfo = (CacheTaskInfo)task;
            foreach (var data in taskInfo.CacheData)
            {
                var sourceData = Utilities.DecodeData(data.SourceType, data.Source);
                var regexStr = data.Regex;
                var cacheKey = data.CacheKey;
                var regex = new Regex(regexStr, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                var match = regex.Match(sourceData);
                taskResult.Success = match.Success;
                Storage.Cache.Add(cacheKey, match.Value);
                if (!taskResult.Success)
                {
                    Logger.Info($"Failed! No match found for '{regexStr}' on source '{sourceData}'");
                    return taskResult.Failed();
                }
                Logger.Info($"Success! Match found for '{regexStr}' with value '{match.Value}', stored at '{cacheKey}'");
            }
            return taskResult.Success();
        }
    }

}
