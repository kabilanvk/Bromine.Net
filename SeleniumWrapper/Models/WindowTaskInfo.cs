using Bromine.Automation.Core.Models;
using Newtonsoft.Json;

namespace Bromine.SeleniumWrapper.Models
{
    public class WindowTaskInfo : TaskInfo
    {
        [JsonProperty("Close")]
        public bool Close { get; set; }

        [JsonProperty("Switch")]
        public bool Switch { get; set; }

        [JsonProperty("SwitchToIframe")]
        public bool? SwitchToIframe { get; set; }

        [JsonProperty("FrameId")]
        public string FrameId { get; set; }
    }
}

