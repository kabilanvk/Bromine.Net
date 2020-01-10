using System.Collections.Generic;
using Bromine.Automation.Core.Enum;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bromine.RestServiceWrapper.Models
{
    public class Header
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Request
    {
        public string Url { get; set; }
        public string HttpMethod { get; set; }
        public List<Header> Header { get; set; }
        public object Payload { get; set; }
    }

    public class Response
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public RestResponseType Type { get; set; }
        public object Value { get; set; }
        public int? StatusCode { get; set; }

    }

    public class RestInfo
    {
        public string TestCase { get; set; }
        public Request Request { get; set; }
        public Response Response { get; set; }
    }

}
