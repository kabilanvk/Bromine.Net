using System.Collections.Generic;
using Bromine.Automation.Core.Models;
using Newtonsoft.Json;

namespace Bromine.SeleniumWrapper.Models
{
  public class CookieTaskInfo : TaskInfo
  {
    [JsonProperty("Cks")]
    public Dictionary<string, string> Cookies { get; set; }

  }
}

