using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Helpers;
using Bromine.SeleniumWrapper.Models;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
    public class InputTask : BrowserTask
    {
        public override Task<TaskResult> Process(TaskInfo task)
        {
            if (!(task is InputTaskInfo)) throw new ArgumentException($"Expected InputTaskInfo but passed {task.GetType().Name}");

            var result = new TaskResult(false);
            var existingValue = "";
            var taskInfo = (InputTaskInfo)task;
            foreach (var inputData in taskInfo.Inputs)
            {
                var lookupType = inputData.Type;
                var lookupValue = inputData.Expression;
                var inputValue = inputData.InputValue;
                var skipClearingExistingValue = inputData.SkipClearingExistingValue;

                var element = CurrentBrowser.Driver.WaitAndGetElement(lookupType.LocateBy(lookupValue));
                if (inputData.AppendToExistingValue == true)
                {
                    existingValue = element.GetAttribute("value");
                }
                if (element == null) return result.Result();
                //Ignore exception from CSS
                try
                {
                    if (!skipClearingExistingValue)
                    {
                        element.Clear();
                        Debug("Cleared existing values.");
                    }    
                }
                catch
                {
                    Info("Failed! Error while clearing existing values.");
                }
                if (!string.IsNullOrEmpty(inputValue))
                {
                    element.SendKeys(inputData.AppendToExistingValue ? (existingValue + inputValue) : inputValue);
                }
                Info($"Input '{lookupType}:{lookupValue}' assigned with value '{inputValue}'");
            }

            return result.Success();
        }
    }
}
