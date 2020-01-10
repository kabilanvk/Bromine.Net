using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Helpers;
using Bromine.SeleniumWrapper.Models;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
  public class HandlePromptTask : BrowserTask
  {
    public override Task<TaskResult> Process(TaskInfo task)
    {
      if (!(task is PromptTaskInfo)) throw new ArgumentException($"Expected PromptTaskInfo but passed {task.GetType().Name}");

      var result = new TaskResult(false);
      var taskInfo = (PromptTaskInfo)task;
      var action = taskInfo.ActionType;
      var waitResult = CurrentBrowser.Driver.WaitForModal();
      if (!waitResult)
      {
        Info("Failed! No alert or prompt displayed");
        return result.Failed();
      }
      var prompt = CurrentBrowser.Driver.SwitchTo().Alert();
      var inputValue = taskInfo.InputValue;
      if (!string.IsNullOrEmpty(inputValue))
      {
        prompt.SendKeys(inputValue);
        Info($"Prompt input '{inputValue}' assigned");
      }
      if (action == PromptActionType.Cancel)
      {
        Info("Prompt dismissed");
        prompt.Dismiss();
      }
      else
      {
        Info("Prompt accepted");
        prompt.Accept();
      }

      return result.Success();
    }
  }
}
