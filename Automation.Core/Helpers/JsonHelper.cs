using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using log4net;
using Newtonsoft.Json;

namespace Bromine.Automation.Core.Helpers
{
  public class JsonHelper
  {
    private readonly ILog _log = LogHelper.GetLogger();

    static JsonHelper()
    {
      Instance = new JsonHelper();
    }

    public static JsonHelper Instance { get; }

    private string ReadJsonToString(string fileDir)
    {
      var jsonString = string.Empty;
      try
      {
        //Checking if file exists
        if (File.Exists(fileDir))
        {
          //Read json
          jsonString = File.ReadAllText(fileDir);
        }
      }
      catch (Exception e)
      {
        _log.Error(string.Format("Exception while reading json config file. - {0}", e.Message));
      }
      return jsonString;
    }

    public static string SerializeObjectToString(object jsonObject)
    {
      return JsonConvert.SerializeObject(jsonObject);
    }

    public T ConfigureJsonFromFile<T>(string fileDir)
    {
      var jsonString = ReadJsonToString(fileDir);
      return ConfigureJsonFromString<T>(jsonString);
    }

    public T ConfigureJsonFromString<T>(string str)
    {
      try
      {
        if (string.IsNullOrEmpty(str))
          return default(T);
        var jsonConfig = JsonConvert.DeserializeObject<T>(str);
        return jsonConfig;
      }
      catch (Exception e)
      {
        _log.Error(string.Format("Exception while converting json config file as object. - {0}", e.Message));
        return default(T);
      }
    }

    public Dictionary<string, object> ConvertJsonToDictionary(string jsonData)
    {
      object data;
      var dic = new Dictionary<string, object>();
      if (jsonData.StartsWith("["))
      {
        var list = new List<Dictionary<string, object>>();
        var listMatch = Regex.Matches(jsonData, @"{[\s\S]+?}");
        foreach (Match listItem in listMatch)
        {
          list.Add(ConvertJsonToDictionary(listItem.ToString()));
        }
        data = list;
        dic.Add("list", data);
      }
      else
      {
        var match = Regex.Matches(jsonData, @"""(.+?)"": {0,1}(\[[\s\S]+?\]|null|"".+?""|-{0,1}\d*)");
        foreach (Match item in match)
        {
          try
          {
            if (item.Groups[2].ToString().StartsWith("["))
            {
              var list = new List<Dictionary<string, object>>();
              var listMatch = Regex.Matches(item.Groups[2].ToString(), @"{[\s\S]+?}");
              foreach (Match listItem in listMatch)
              {
                list.Add(ConvertJsonToDictionary(listItem.ToString()));
              }
              data = list;
            }
            else if (item.Groups[2].ToString().ToLower() == "null") data = null;
            else data = item.Groups[2].ToString();
            dic.Add(item.Groups[1].ToString(), data);
          }
          catch (Exception ex) { _log.Error(ex); }
        }
      }
      return dic;
    }
  }
}
