using Bromine.Automation.Core.Models;
using Newtonsoft.Json;

namespace Bromine.RestServiceWrapper.Models
{
    public class SilentTaskInfo : TaskInfo
    {
        public Request Request { get; set; }

        public Response Response { get; set; }

        [JsonProperty("Cache")]
        public string CacheKey { get; set; }
    }
}
