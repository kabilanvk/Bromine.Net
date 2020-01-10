using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bromine.SeleniumWrapper.Models
{
  public class PromptTaskInfo : TaskInfo
  {
    [JsonProperty("Proceed")]
    [JsonConverter(typeof(StringEnumConverter))]
    public PromptActionType ActionType { get; set; }

    [JsonProperty("Input")]
    public string InputValue { get; set; }
  }
}

