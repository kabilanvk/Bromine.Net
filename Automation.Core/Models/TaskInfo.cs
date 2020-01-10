using System.Collections.Generic;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Enum;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bromine.Automation.Core.Models
{
  public class TaskInfo
  {
    [NoCacheRequired]
    public ActionType Action { get; set; }

    [NoCacheRequired]
    [JsonProperty("Env", ItemConverterType = typeof(StringEnumConverter))]
    public List<EnvironmentType> Environments { get; set; }

    [NoCacheRequired]
    public string Comment { get; set; }

    [NoCacheRequired]
    public bool IsNegated { get; set; }

    [NoCacheRequired]
    public string TaskName { get; set; }

    [NoCacheRequired]
    public TestInfo Parent { get; set; }
  }
}
