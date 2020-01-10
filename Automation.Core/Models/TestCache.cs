using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Extensions;

namespace Bromine.Automation.Core.Models
{
    public class TestCache
    {
        private const string KeyPattern = @"\@([^#]*)\#";

        private readonly IDictionary<string, string> _cacheDic = new Dictionary<string, string>();

        public void Add(string key, string value)
        {
            if (!IsValid(ref key)) return;
            lock (_cacheDic)
            {
                _cacheDic.Add(key, value);
            }
        }

        //CC-5302, for session storage update, the latest log will always cover the existing log.
        public void AddOrUpdate(string key, string value)
        {
            if (!IsValid(ref key)) return;
            lock (_cacheDic)
            {
                _cacheDic[key] = value;
            }
        }

        public string Get(string key)
        {
            if (!IsValid(ref key)) return null;
            lock (_cacheDic)
            {
                return _cacheDic.ContainsKey(key) ? _cacheDic[key] : null;
            }
        }

        public bool Contains(string key)
        {
            if (!IsValid(ref key)) return false;
            lock (_cacheDic)
            {
                return _cacheDic.ContainsKey(key);
            }
        }

        public void Remove(string key)
        {
            if (!IsValid(ref key)) return;
            lock (_cacheDic)
            {
                _cacheDic.Remove(key);
            }
        }

        public string ReplaceCacheKeys(string data)
        {
            lock (_cacheDic)
            {
                foreach (var cacheKey in _cacheDic.Keys)
                {
                    var parameterToReplace = cacheKey + "#";
                    if (data.Contains(parameterToReplace))
                    {
                        data = data.Replace(parameterToReplace, _cacheDic[cacheKey]);
                    }
                }
                return data;
            }
        }

        public void ReplaceCacheKeys(TaskInfo taskInfo)
        {
            ReplaceCacheKeysInternal(taskInfo);
        }

        public override string ToString()
        {
            lock (_cacheDic)
            {
                return string.Join(",", _cacheDic.Where(k => !k.Key.Equals(Constants.Log)).Select(kv => $"{kv.Key}={kv.Value}"));
            }
        }

        #region Private Methods

        private static bool IsValid(ref string key)
        {
            if (key.IsNotEmpty()) key = key.TrimEnd('#');
            if (key.IsEmpty()) return false;
            if (!key.StartsWith("@")) key = $"@{key}";
            return true;
        }

        private void ReplaceCacheKeysInternal(object taskInfo)
        {
            var properties = taskInfo.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                if (prop.GetCustomAttribute(typeof(NoCacheRequired)) != null) continue;
                var propValue = prop.GetValue(taskInfo);
                if (propValue == null) continue;

                if (propValue is string)
                {
                    prop.SetValue(taskInfo, GetCacheValue(propValue.ToString()));
                }
                else if (propValue is List<string>)
                {
                    var strList = propValue as List<string>;
                    for (var index = 0; index < strList.Count; index++)
                    {
                        strList[index] = GetCacheValue(strList[index]);
                    }
                    prop.SetValue(taskInfo, strList);
                }
                else if (propValue is IDictionary<string, string>)
                {
                    prop.SetValue(taskInfo, ((IDictionary<string, string>)propValue)
                      .ToDictionary(k => GetCacheValue(k.Key), k => GetCacheValue(k.Value)));
                }
                else if (propValue is IDictionary<string, LocateByType>)
                {
                    prop.SetValue(taskInfo, ((IDictionary<string, LocateByType>)propValue)
                      .ToDictionary(k => GetCacheValue(k.Key), k => k.Value));
                }
                else if (propValue.GetType().IsClass)
                {
                    if (propValue is IEnumerable)
                    {
                        foreach (var obj in (IEnumerable)propValue)
                        {
                            ReplaceCacheKeysInternal(obj);
                        }
                    }
                    else
                    {
                        ReplaceCacheKeysInternal(propValue);
                    }
                }
            }
        }

        private string GetCacheValue(string key)
        {
            if (key.IsEmpty() || !key.Contains("@")) return key;
            if (key.StartsWith("@") && Contains(key))
            {
                var valueFromCache = Get(key);
                return valueFromCache.IsEmpty() ? string.Empty : AssignFinalCacheValue(valueFromCache);
            }
            var matches = Regex.Matches(key, KeyPattern);
            if (matches.Count <= 0) return key;
            foreach (Match match in matches)
            {
                var matchedKey = match.Value.TrimEnd('#');
                if (matchedKey.IsNotEmpty() && Contains(matchedKey))
                {
                    key = key.Replace(matchedKey + "#", AssignFinalCacheValue(Get(matchedKey)));
                }
            }
            return key;
        }

        //Get one more level of caching		
        private string AssignFinalCacheValue(string cacheValue)
        {
            return cacheValue.StartsWith("@") ? Get(cacheValue) : cacheValue;
        }

        #endregion
    }
}
