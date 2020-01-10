using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Helpers;
using Bromine.SeleniumWrapper.Models;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
    public class VerifyAttributesTask : BrowserTask
    {
        public override Task<TaskResult> Process(TaskInfo task)
        {
            if (!(task is ElementTaskInfo)) throw new ArgumentException($"Expected ElementTaskInfo but passed {task.GetType().Name}");

            var result = new TaskResult(false);
            var taskInfo = (ElementTaskInfo)task;
            foreach (var ele in taskInfo.Elements)
            {
                var lookupType = ele.Type;
                var lookupValue = ele.Expression;
                var attribute = ele.Attribute;
                var attributeValue = ele.AttributeValue;
                var matchType = ele.Match;

                var response = CurrentBrowser.Driver.GetAttributeValueFromElement(lookupType.LocateBy(lookupValue), attribute);
                if (string.IsNullOrEmpty(response) || string.IsNullOrEmpty(attributeValue) ||
                    (matchType == MatchType.Exact && !response.Equals(attributeValue, StringComparison.InvariantCultureIgnoreCase)))
                {
                    Info($"Failed! Attribute '{lookupValue}:{attributeValue}' with value '{response}' doesn't match with given '{attributeValue}'");
                    return result.Failed();
                }
                if (response.IndexOf(attributeValue, StringComparison.InvariantCultureIgnoreCase) >= 0) continue;
                Info($"Failed! Attribute '{lookupValue}:{attributeValue}' with value '{response}' doesn't contains the given '{attributeValue}'");
                return result.Failed();
            }
            return result.Success();
        }
    }
}
