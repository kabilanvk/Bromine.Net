using System;
using System.Collections.Generic;
using System.Linq;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Models;
using Bromine.GenericWrapper.Models;
using Bromine.RestServiceWrapper.Models;
using Bromine.SeleniumWrapper.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bromine.TestRunner.Helpers
{
  public class TestJsonConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return typeof(List<TaskActionInfo>).IsAssignableFrom(objectType);
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      var objArray = JArray.Load(reader);
      var taskActionInfos = new List<TaskActionInfo>();
      foreach (var obj in objArray.Children())
      {
        var property = obj.Children<JProperty>().First();
        var action = (ActionType)Enum.Parse(typeof(ActionType), property.Name.TrimStart('!'), true);
        var taskData = CreateObject(action);
        if (taskData == null) continue;
        taskData.Action = action;
        taskData.IsNegated = property.Name.StartsWith("!");
        serializer.Populate(property.Value.CreateReader(), taskData);
        taskActionInfos.Add(new TaskActionInfo { Action = action, Task = taskData });
      }
      return taskActionInfos;
    }

    private static TaskInfo CreateObject(ActionType action)
    {
      TaskInfo data;
      switch (action)
      {
        case ActionType.SilentRequest:
          data = new SilentTaskInfo();
          break;

        case ActionType.AddTemplate:
          data = new TemplateTaskInfo();
          break;
        case ActionType.CompareValue:
          data = new CompareTaskInfo();
          break;
       
        case ActionType.GetValueFromCache:
        case ActionType.ClearCacheValue:
        case ActionType.ExtractStringFromUrl:
        case ActionType.RewriteUrl:
        case ActionType.ReadValueFromLog:
          data = new CacheTaskInfo();
          break;

        case ActionType.Navigate:
          data = new NavigateTaskInfo();
          break;
        case ActionType.SwitchContextToIframe:
          data = new WindowTaskInfo();
          break;
        case ActionType.Wait:
        case ActionType.WaitUntil:
        case ActionType.MatchLog:
        case ActionType.MatchByRegex:
          data = new LogTaskInfo();
          break;
        case ActionType.VerifyUi:
        case ActionType.Click:
          data = new UiTaskInfo();
          break;
        case ActionType.ExecuteJavaScript:
          data = new ScriptTaskInfo();
          break;
        case ActionType.TakeScreenShot:
        case ActionType.NavigateBack:
        case ActionType.Refresh:
          data = new TaskInfo();
          break;
        case ActionType.VerifyUrl:
          data = new VerifyUrlTaskInfo();
          break;
        case ActionType.SwitchWindow:
        case ActionType.CloseWindow:
          data = new WindowTaskInfo();
          break;
        case ActionType.Input:
        case ActionType.InputEmail:
        case ActionType.SelectDropdown:
          data = new InputTaskInfo();
          break;
        case ActionType.UlSelectDropdown:
          data = new UlSelectTaskInfo();
          break;
        case ActionType.AddCookie:
        case ActionType.DeleteCookie:
        case ActionType.VerifyCookie:
          data = new CookieTaskInfo();
          break;
        case ActionType.FetchDataFromUi:
        case ActionType.VerifyAttributes:
        case ActionType.VerifyCssProperties:
          data = new ElementTaskInfo();
          break;
        case ActionType.HandlePrompt:
          data = new PromptTaskInfo();
          break;
        case ActionType.DecodeUrl:
          data = new DecodeUrlTaskInfo();
          break;

        case ActionType.AddAuthHeader:
          data = new AddAuthHeaderTaskInfo();
          break;
        default:
          throw new NotImplementedException($"Unknown action '{action}'");
      }
      return data;
    }
  }
}
