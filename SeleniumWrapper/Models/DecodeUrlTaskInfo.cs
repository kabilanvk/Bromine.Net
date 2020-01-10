using Bromine.Automation.Core.Models;
using Newtonsoft.Json;

namespace Bromine.SeleniumWrapper.Models
{
  public class DecodeUrlTaskInfo : TaskInfo
  {
    [JsonProperty("Url")]
    public string Url { get; set; }

    [JsonProperty("Cache")]
    public string CacheKey { get; set; }
  }
}

