using System;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.RegularExpressions;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Enum;

namespace Bromine.Automation.Core.Helpers
{
    public static class Utilities
    {
        public static string GetCurrentTimeStamp()
        {
            // for further prescison in time use format string: " hh:mm:ss.fff tt"
            return DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        }

        public static string ConfigureUrl(string rawUrl)
        {
            foreach (var key in Constants.AppConfig.KnownUrls.Keys)
            {
                rawUrl = Regex.Replace(rawUrl, $"{{#{key}}}(\\/?)", Constants.AppConfig.KnownUrls[key], RegexOptions.IgnoreCase);
            }

            return rawUrl;
        }

        public static string ConvertToAppConfigValue(string typeName)
        {
            var appconfigType = typeof(Constants.AppConfig).GetProperty(typeName);
            if (appconfigType == null) return string.Empty;
            var stringValue = appconfigType.GetValue(null, null);
            return stringValue.ToString();
        }

        public static string ReplaceStringOfMultipleVariable(string rawString)
        {
            var pattern = @"\{#[a-zA-Z0-9]*\}";
            var matches = Regex.Matches(rawString, pattern);
            foreach (var match in matches)
            {
                var typeName = match.ToString().Replace("{#", "").Replace("}", "").Trim();
                var replacementValue = ConvertToAppConfigValue(typeName);
                rawString = rawString.Replace(match.ToString(), replacementValue);
            }
            return rawString;
        }

        public static string SanitizeUrl(string rawUrl, bool isConvertedToLower = false)
        {
            var uri = new Uri(rawUrl);
            var protocal = uri.Scheme;
            var domain = uri.Host;
            var urlSubstring = uri.PathAndQuery;
            urlSubstring = urlSubstring.Replace("//", "/");
            var baseUrl = new Uri($"{protocal}://{domain}");
            var url = new Uri(baseUrl, urlSubstring);
            return !isConvertedToLower ? url.ToString().ToLower() : url.ToString();
        }

        public static string RemoveReservedCharacters(string url)
        {
            //Fix the url by removing the reserved '&' from the normal string.
            url = url.Replace("&%", "%26");
            url = url.Replace("&#", "%26#");
            return url;
        }

        public static NameValueCollection ParseQueryString(string s)
        {
            var logger = LogHelper.GetLogger();
            var collection = new NameValueCollection();
            //to retrieve the querystring
            if (s.Contains("?"))
            {
                s = s.Substring(s.IndexOf('?') + 1);
                logger.Debug($"Query string to parse: {s}");
            }

            foreach (var parameter in Regex.Split(s, "&"))
            {
                var index = parameter.IndexOf("=", StringComparison.OrdinalIgnoreCase);
                var key = parameter.Substring(0, index);
                var val = parameter.Substring(index + 1);
                collection.Add(key, val);
            }
            return collection;
        }


        public static void SetPropertyValue<T>(string propertyName, object value, T propertyObject)
        {
            var browserHeaderProperty = typeof(T).GetProperty(propertyName);
            if (browserHeaderProperty != null) browserHeaderProperty.SetValue(propertyObject, value);
        }

        public static bool EqualsIgnoreCase(dynamic source, dynamic compare)
        {
            if (source == null && compare == null) return true;
            if (source == null || compare == null) return false;
            return source.Equals(compare);
        }

        public static string DecodeData(DataType type, string source)
        {
            switch (type)
            {
                case DataType.Base64:
                    return Encoding.UTF8.GetString(Convert.FromBase64String(source));
                case DataType.Base64Url:
                    return Encoding.UTF8.GetString(Base64UrlDecode(source));
                case DataType.Jwt:
                    return (new JwtSecurityTokenHandler().ReadToken(source) as JwtSecurityToken)?.RawData;
                default:
                    return source;
            }
        }

        public static byte[] Base64UrlDecode(string input)
        {
            var s = input.Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4)
            {
                case 0:
                    return Convert.FromBase64String(s);
                case 2:
                    s += "==";
                    goto case 0;
                case 3:
                    s += "=";
                    goto case 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(input), "Illegal base64url string!");
            }
        }
    }
}
