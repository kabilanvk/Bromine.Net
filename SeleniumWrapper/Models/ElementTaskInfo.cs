using System.Collections.Generic;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bromine.SeleniumWrapper.Models
{
  public class ElementTaskInfo : TaskInfo
  {
    [JsonProperty("Ele")]
    public List<Element> Elements { get; set; }
  }

  public class Element
  {
    [JsonProperty("Type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public LocateByType Type { get; set; }

    [JsonProperty("Expr")]
    public string Expression { get; set; }

    [JsonProperty("Attr")]
    public string Attribute { get; set; }

    [JsonProperty("AttrVal")]
    public string AttributeValue { get; set; }

    [JsonProperty("Css")]
    public string CssProperty { get; set; }

    [JsonProperty("CssVal")]
    public string CssValue { get; set; }

    [JsonProperty("Match")]
    [JsonConverter(typeof(StringEnumConverter))]
    public MatchType Match { get; set; }

    [JsonProperty("Cache")]
    public string CacheKey { get; set; }
  }
}

