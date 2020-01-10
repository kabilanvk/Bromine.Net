using System.Collections.Generic;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bromine.SeleniumWrapper.Models
{
    public class UlSelectTaskInfo : TaskInfo
    {
        [JsonProperty("Ele")]
        public List<UlSelectData> SelectData { get; set; }
    }

    public class UlSelectData
    {
        [JsonProperty("UlType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LocateByType UlType { get; set; }

        [JsonProperty("UlExpr")]
        public string UlExpression { get; set; }

        [JsonProperty("LinkType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LocateByType LinkType { get; set; }

        [JsonProperty("LinkExpr")]
        public string LinkExpression { get; set; }
    }
}
