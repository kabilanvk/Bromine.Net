using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.GenericWrapper.Models;

namespace Bromine.GenericWrapper.Tasks
{
    public class CompareValueTask : BaseTask
    {
        public override Task<TaskResult> Process(TaskInfo task)
        {
            if (!(task is CompareTaskInfo)) throw new ArgumentException($"Expected CompareTaskInfo but passed {task.GetType().Name}");

            var result = new TaskResult(false);
            var taskInfo = (CompareTaskInfo)task;
            //TODO: handle negated case
            if (task.IsNegated) return result.Result();

            foreach (var data in taskInfo.CompareData)
            {
                var source = data.Source;
                var expected = data.CompareWith;
                var regexPattern = data.Regex;
                if (string.IsNullOrEmpty(source))
                {
                    if (!string.IsNullOrEmpty(expected))
                    {
                        Logger.Info($"Expected '{expected}' but actual is null");
                        return result.Failed();
                    }
                    Logger.Info("Both values are null as expected");
                    continue;
                }
                if (!string.IsNullOrEmpty(regexPattern))
                {
                    if (!Regex.IsMatch(source, regexPattern, RegexOptions.IgnoreCase))
                    {
                        Logger.Info($"Regex '{regexPattern}' failed on source '{source}'");
                        return result.Failed();
                    }
                    Logger.Info($"Regex '{regexPattern}' succeeded on source '{source}'");
                    continue;
                }
                if (expected.Equals("*"))
                {
                    Logger.Info($"Matched any value for '{source}'");
                    continue; //wild card, the value can be any, then move on to next iteration
                }

                if (source.Equals(expected, StringComparison.OrdinalIgnoreCase)) continue;
                Logger.Info($"Expected '{expected}' but actual '{source}'");
                return result.Failed();
            }

            return result.Success();
        }
    }
}
