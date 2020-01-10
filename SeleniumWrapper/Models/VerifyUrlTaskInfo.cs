using System.Collections.Generic;
using Bromine.Automation.Core.Models;
using Newtonsoft.Json;

namespace Bromine.SeleniumWrapper.Models
{
  public class VerifyUrlTaskInfo : TaskInfo
  {
    [JsonProperty("Urls")]
    public List<string> Urls { get; set; }
  }
}

