using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
    public class RewriteUrlTask : BrowserTask
    {
        public override Task<TaskResult> Process(TaskInfo task)
        {
            if (!(task is CacheTaskInfo)) throw new ArgumentException($"Expected CacheTaskInfo but passed {task.GetType().Name}");

            var result = new TaskResult(false);
            var taskInfo = (CacheTaskInfo)task;
            foreach (var data in taskInfo.CacheData)
            {
                var regexStr = data.Regex;
                var cacheParam = data.CacheKey;
                var replaceKey = data.LookupKey;
                var regex = new Regex(regexStr, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                var match = regex.Match(CurrentBrowser.Driver.Url);
                result.Success = match.Success != task.IsNegated;
                var url = CurrentBrowser.Driver.Url;
                if (!match.Success)
                {
                    Info($"Match not found in url '{url}' with regex'{regex}'");
                    return result.Failed();
                }
                url = url.Replace(match.Value, replaceKey);
                Storage.Cache.Add(cacheParam, url);
                Info($"Url replaced with value '{cacheParam}:{url}' and cached");
            }

            return result.Success();
        }
    }
}
