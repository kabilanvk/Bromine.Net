using System;
using System.Threading.Tasks;
using System.Web;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Models;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
  public class DecodeUrlTask : BrowserTask
  {
    public override Task<TaskResult> Process(TaskInfo task)
    {
      if (!(task is DecodeUrlTaskInfo)) throw new ArgumentException($"Expected DecodeUrlTaskInfo but passed {task.GetType().Name}");

      var result = new TaskResult(false);
      var taskInfo = (DecodeUrlTaskInfo)task;
      var cacheParam = taskInfo.CacheKey;
      if (string.IsNullOrEmpty(cacheParam))
      {
        Info("Cache key is missing.");
        return result.Failed();
      }
      //if EncodedUrl is specified, use that as the input url, otherwise use the current url
      var url = taskInfo.Url ?? CurrentBrowser.Driver.Url;
      var decode = HttpUtility.UrlDecode(url);
      result.Success = decode != null != task.IsNegated;
      Storage.Cache.Add(cacheParam, decode);
      return result.Result();
    }
  }
}
