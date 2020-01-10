using System.Collections.Generic;
using Bromine.Automation.Core.Enum;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bromine.Automation.Core.Models
{
    public class CacheTaskInfo : TaskInfo
    {
        [JsonProperty("Data")]
        public List<CacheData> CacheData { get; set; }
    }

    public class CacheData
    {
        [JsonProperty("Expr")]
        public string LookupKey { get; set; }

        [JsonProperty("Cache")]
        public string CacheKey { get; set; }

        [JsonProperty("Src")]
        public string Source { get; set; }

        [JsonProperty("SrcType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DataType SourceType { get; set; }

        [JsonProperty("Regex")]
        public string Regex { get; set; }
    }
}
