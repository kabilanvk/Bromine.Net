using System.Collections.Generic;
using Bromine.Automation.Core.Models;
using Newtonsoft.Json;

namespace Bromine.SeleniumWrapper.Models
{
  public class LogTaskInfo : TaskInfo
  {
    [JsonProperty("Log")]
    public string Log { get; set; }

    [JsonProperty("Occurrence")]
    public int Occurrence { get; set; }

    [JsonProperty("Timeout")]
    public int Timeout { get; set; }

    [JsonProperty("Ordered")]
    public List<string> Ordered { get; set; }

    [JsonProperty("UnOrdered")]
    public List<string> UnOrdered { get; set; }

    [JsonProperty("Regex")]
    public List<string> Regex { get; set; }
  }
}
