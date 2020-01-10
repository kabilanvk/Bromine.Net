using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Helpers;
using Bromine.SeleniumWrapper.Models;
using OpenQA.Selenium;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
    public class VerifyCssPropertiesTask : BrowserTask
    {
        public override Task<TaskResult> Process(TaskInfo task)
        {
            if (!(task is ElementTaskInfo)) throw new ArgumentException($"Expected ElementTaskInfo but passed {task.GetType().Name}");

            var result = new TaskResult(false);
            var taskInfo = (ElementTaskInfo)task;
            foreach (var ele in taskInfo.Elements)
            {
                var lookupValue = ele.Expression;
                var cssProperty = ele.CssProperty;
                var cssValue = ele.CssValue;
                var matchType = ele.Match;

                var response = CurrentBrowser.Driver.GetCssProperties(By.CssSelector(lookupValue), cssProperty);
                if (string.IsNullOrEmpty(response) || string.IsNullOrEmpty(cssValue) ||
                    (matchType == MatchType.Exact && !response.Equals(cssValue, StringComparison.InvariantCultureIgnoreCase)))
                {
                    Info($"Failed! Css property '{lookupValue}:{cssProperty}' with value '{response}' doesn't match exactly with given '{cssValue}'.");
                    return result.Failed();
                }
                if (response.IndexOf(cssValue, StringComparison.InvariantCultureIgnoreCase) >= 0) continue;
                Info($"Failed! Css property '{lookupValue}:{cssProperty}' with value '{response}' doesn't contains the given '{cssValue}'.");
                return result.Failed();
            }

            return result.Success();
        }
    }
}
