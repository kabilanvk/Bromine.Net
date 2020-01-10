using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Models;

namespace Bromine.Automation.Core.Extensions
{
  public static class CoreExtensions
  {
    public static bool IsEmpty(this string value)
    {
      return string.IsNullOrWhiteSpace(value);
    }
    public static bool IsEqualTo(this string string1, string string2)
    {
      if (string1 == null && string2 == null) return true;
      if (string1 == null || string2 == null) return false;
      return string1.Equals(string2, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsNotEmpty(this string str)
    {
      return !str.IsEmpty();
    }

    public static void SafeAdd<T, TU>(this Dictionary<T, TU> dict, T key, TU value)
    {
      if (dict == null) return;

      if (dict.ContainsKey(key))
        dict[key] = value;
      else
        dict.Add(key, value);
    }

    public static Task<TaskResult> Success(this TaskResult result)
    {
      result.Success = true;
      return result.Result();
    }

    public static Task<TaskResult> Failed(this TaskResult result)
    {
      result.Success = false;
      return result.Result();
    }

    public static Task<TaskResult> Result(this TaskResult result)
    {
      return Task.FromResult(result);
    }
    
    public static bool EqualsIgnoreCase(this string source, string compare)
    {
      if (string.IsNullOrEmpty(source)) return string.IsNullOrEmpty(compare);
      return source.Equals("*") || source.Equals(compare, StringComparison.OrdinalIgnoreCase);
    }

    public static bool EqualsIgnoreCase(this Dictionary<string, string> source, Dictionary<string, string> compare)
    {
      if ((source == null || source.Count == 0) && (compare == null || compare.Count == 0)) return true;
      if ((source == null || source.Count == 0) || (compare == null || compare.Count == 0)) return false;
      return source.All(kv => kv.Value.Equals(compare[kv.Key], StringComparison.OrdinalIgnoreCase));
    }

    public static bool EqualsOrAny(this string source, string compare)
    {
      if (string.IsNullOrEmpty(source)) return string.IsNullOrEmpty(compare);
      return source.Equals("*") || source.Equals(compare);
    }

    public static string ToJson(this Dictionary<string, string> dictionary)
    {
      var kvs = dictionary.Select(kvp => $"{{\'key':\'{kvp.Key}\',\'value':\'{string.Join(",", kvp.Value)}'}}");
      return string.Concat("[", string.Join(",", kvs), "]");
    }

    public static string ToJson(this Dictionary<string, object> dictionary)
    {
      var kvs = dictionary.Select(kvp => $"{{\'{kvp.Key}\':\'{kvp.Value}'}}");
      return $"[{string.Join(",", kvs)}]";
    }

    public static string GetPlugin(this TestInfo testInfo)
    {
      var jsonFile = Directory.GetFiles(Path.Combine(Constants.AppConfig.TestDataBasePath, "Testcases"),
        testInfo.Name + ".json", SearchOption.AllDirectories);
      if (jsonFile.Length <= 0) return string.Empty;
      var dirPath = Path.GetDirectoryName(jsonFile[0]);
      return !string.IsNullOrEmpty(dirPath) ? new DirectoryInfo(dirPath).Name : string.Empty;
    }
  }
}
