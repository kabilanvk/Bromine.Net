using System.Collections.Generic;
using Bromine.Automation.Core.Models;
using Newtonsoft.Json;

namespace Bromine.GenericWrapper.Models
{
  public class CompareTaskInfo : TaskInfo
  {
    [JsonProperty("Data")]
    public List<CompareData> CompareData { get; set; }
  }

  public class CompareData
  {
    [JsonProperty("Src")]
    public string Source { get; set; }

    [JsonProperty("Compare")]
    public string CompareWith { get; set; }

    [JsonProperty("Regex")]
    public string Regex { get; set; }
  }
}
