using System.Collections.Generic;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bromine.SeleniumWrapper.Models
{
  public class UiTaskInfo : TaskInfo
  {
    [JsonProperty("Ele", ItemConverterType = typeof(StringEnumConverter))]
    public Dictionary<string, LocateByType> Elements { get; set; }
  }
}

