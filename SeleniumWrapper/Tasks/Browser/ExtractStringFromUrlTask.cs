using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
  public class ExtractStringFromUrlTask : BrowserTask
  {
    public override Task<TaskResult> Process(TaskInfo task)
    {
      if (!(task is CacheTaskInfo)) throw new ArgumentException($"Expected CacheTaskInfo but passed {task.GetType().Name}");

      var result = new TaskResult(false);
      var taskInfo = (CacheTaskInfo)task;
      foreach (var data in taskInfo.CacheData)
      {
        var source = data.Source ?? CurrentBrowser.Driver.Url;
        var lookupKey = data.LookupKey;
        var cacheParameter = data.CacheKey;
        var queryParam = HttpHelper.Value.GetQueryStringDictionary(source);

        if (string.IsNullOrEmpty(lookupKey))
        {
          Info("No lookupKey parameter found.");
          return result.Failed();
        }
        if (!queryParam.ContainsKey(lookupKey))
        {
          Info($"LookupKey '{lookupKey}' does not exist from source '{source}'");
          return result.Failed();
        }
        Storage.Cache.Add(cacheParameter, queryParam[lookupKey]);
        Info($"LookupKey found '{queryParam[lookupKey]}' from source '{source}' and cached");
      }
      return result.Success();
    }
  }
}
