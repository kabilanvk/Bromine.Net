using System.Collections.Generic;
using Bromine.Automation.Core.Models;
using Newtonsoft.Json;

namespace Bromine.SeleniumWrapper.Models
{
  public class ScriptTaskInfo : TaskInfo
  {
    [JsonProperty("Script")]
    public List<ScriptData> Expressions { get; set; }
  }

  public class ScriptData
  {
    [JsonProperty("Expr")]
    public string Expression { get; set; }

    [JsonProperty("Cache")]
    public string CacheKey { get; set; }
  }

}

