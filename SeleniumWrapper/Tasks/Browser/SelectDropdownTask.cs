using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Helpers;
using Bromine.SeleniumWrapper.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
    public class SelectDropdownTask : BrowserTask
    {
        public override Task<TaskResult> Process(TaskInfo task)
        {
            if (!(task is InputTaskInfo)) throw new ArgumentException($"Expected InputTaskInfo but passed {task.GetType().Name}");

            var result = new TaskResult(false);
            var taskInfo = (InputTaskInfo)task;
            foreach (var inputData in taskInfo.Inputs)
            {
                var lookupType = inputData.Type;
                var lookupValue = inputData.Expression;
                var inputValue = inputData.InputValue;
                var element = CurrentBrowser.Driver.WaitAndGetElement(lookupType.LocateBy(lookupValue));
                if (element == null) return result.Result();
                if (!string.IsNullOrEmpty(inputValue))
                {
                    new SelectElement(element).SelectByValue(inputValue);
                    //element.SendKeys(inputValue);
                }
                Info($"Input '{lookupType}:{lookupValue}' assigned with value '{inputValue}'");
            }

            return result.Success();
        }
    }

    public class UlSelectDropdownTask : BrowserTask
    {
        public override Task<TaskResult> Process(TaskInfo task)
        {
            if (!(task is UlSelectTaskInfo)) throw new ArgumentException($"Expected InputTaskInfo but passed {task.GetType().Name}");

            var result = new TaskResult(false);
            var taskInfo = (UlSelectTaskInfo)task;
            foreach (var inputData in taskInfo.SelectData)
            {
                var lookupType = inputData.UlType;
                var lookupValue = inputData.UlExpression;
                var lookupLinkValue = inputData.LinkExpression;
                var lookupLinkType = inputData.LinkType;

                var element = CurrentBrowser.Driver.WaitAndGetElement(lookupType.LocateBy(lookupValue));
                if (element == null) return result.Failed();
                element.Click();
                var childElement = element.FindElement(lookupLinkType.LocateBy(lookupLinkValue));
                childElement.SendKeys(Keys.Enter);
            }
            return result.Success();
        }
    }
}
