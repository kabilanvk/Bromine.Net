using Bromine.Automation.Core.Models;
using Newtonsoft.Json;

namespace Bromine.SeleniumWrapper.Models
{
  public class NavigateTaskInfo : TaskInfo
  {
    [JsonProperty("Url")]
    public string Url { get; set; }
  }
}

