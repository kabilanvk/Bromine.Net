using System.Collections.Generic;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bromine.SeleniumWrapper.Models
{
    public class InputTaskInfo : TaskInfo
    {
        [JsonProperty("Ele")]
        public List<InputData> Inputs { get; set; }
    }

    public class InputData
    {
        [JsonProperty("Type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LocateByType Type { get; set; }

        [JsonProperty("Expr")]
        public string Expression { get; set; }

        [JsonProperty("Input")]
        public string InputValue { get; set; }

        [JsonProperty("AppendToExistingValue")]
        public bool AppendToExistingValue { get; set; }

        [JsonProperty("SkipClearingExistingValue")]
        public bool SkipClearingExistingValue { get; set; }
    }
}

