using System;
using System.Collections.Generic;
using System.Linq;
using Bromine.Automation.Core.Helpers;

namespace Bromine.Automation.Core.Models
{
    public class BrowserHeader
    {
        public string UserAgent { get; set; }
        public string CustomUserAgent { get; set; }
        public string XForwardedFor { get; set; }
        public string Authorization { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var browserheader = (BrowserHeader)obj;
            return (UserAgent == browserheader.UserAgent) &&
                (CustomUserAgent == browserheader.CustomUserAgent) &&
                (XForwardedFor == browserheader.XForwardedFor) &&
                (Authorization == browserheader.Authorization);
        }

        public override string ToString()
        {
            return $"UA:{UserAgent},CustomUA:{CustomUserAgent},XF:{XForwardedFor}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public Dictionary<string, string> GetDictionary()
        {
            var headerDic = new Dictionary<string, string>(4);
            AddHeader(headerDic, GetHeaderName("UserAgent"), UserAgent);
            AddHeader(headerDic, GetHeaderName("XForwardedFor"), XForwardedFor);
            AddHeader(headerDic, GetHeaderName("Authorization"), Authorization);
            return headerDic;
        }

        private static string GetHeaderName(string name)
        {
            return TestCore.TestConfiguration.HeaderConfiguration.HeaderProperties
              .FirstOrDefault(ch => ch.Key.Equals(name, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ch.HeaderName))?.HeaderName;
        }

        private static void AddHeader(IDictionary<string, string> dictionary, string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value)) dictionary.Add(key, value);
        }
    }
}
