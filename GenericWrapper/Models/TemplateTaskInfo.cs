using System.Collections.Generic;
using Bromine.Automation.Core.Models;
using Newtonsoft.Json;

namespace Bromine.GenericWrapper.Models
{
  public class TemplateTaskInfo : TaskInfo
  {
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Params")]
    public Dictionary<string, string> Parameters { get; set; }

  }
}
