using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;

namespace Bromine.SeleniumWrapper.Tasks.ConsoleLog
{
    public class ReadValueFromLogTask : BrowserTask
    {
        public override Task<TaskResult> Process(TaskInfo task)
        {
            if (!(task is CacheTaskInfo)) throw new ArgumentException($"Expected CacheTaskInfo but passed {task.GetType().Name}");

            var result = new TaskResult(false);
            var taskInfo = (CacheTaskInfo)task;
            var logs = ConsoleLogHelper.GetConsoleLog();
            if (string.IsNullOrEmpty(logs)) return result.Result();
            foreach (var cacheData in taskInfo.CacheData)
            {
                var regexStr = cacheData.Regex;
                var cacheKey = cacheData.CacheKey;
                var match = Regex.Match(logs, regexStr, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var value = match.Groups["value"].Value;
                    Info($"Success! match found for regex '{regexStr}' with value '{value}' and cached");
                    Storage.Cache.Add(cacheKey, value);
                }
                else
                {
                    Info($"Failed! match not found for regex {regexStr}.");
                    return result.Result();
                }
            }

            return result.Success();
        }
    }
}

