using System;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Helpers;
using Bromine.SeleniumWrapper.Models;
using OpenQA.Selenium;

namespace Bromine.SeleniumWrapper.Tasks.Browser
{
    public class WindowTask : BrowserTask
    {
        public override Task<TaskResult> Process(TaskInfo task)
        {
            if (!(task is WindowTaskInfo)) throw new ArgumentException($"Expected WindowTaskInfo but passed {task.GetType().Name}");

            var result = new TaskResult(false);
            var taskInfo = (WindowTaskInfo)task;
            if (taskInfo.Close)
            {
                SeleniumHelper.CloseOtherWindows(CurrentBrowser.Driver, CurrentBrowser.CurrentWindow);
                return result.Success();
            }

            if (taskInfo.SwitchToIframe.HasValue && taskInfo.SwitchToIframe.Value)
            {
                CurrentBrowser.Driver.SwitchTo().Frame(CurrentBrowser.Driver.FindElement(By.Id(taskInfo.FrameId)));
                return result.Success();
            }

            if(taskInfo.SwitchToIframe.HasValue && taskInfo.SwitchToIframe.Value == false)
            {
                CurrentBrowser.Driver.SwitchTo().DefaultContent();
                return result.Success();
            }

            var windowHandles = CurrentBrowser.Driver.WindowHandles;
            if (windowHandles.Count < 1) return result.Failed();
            if (windowHandles.Count == 1) return result.Success(); //one window no switch, return true
            var currentWindowHandle = CurrentBrowser.Driver.CurrentWindowHandle;
            var popupWindowHandle = string.Empty;
            foreach (var handle in windowHandles)
            {
                if (string.Compare(handle, currentWindowHandle, StringComparison.OrdinalIgnoreCase) == 0) continue;
                popupWindowHandle = handle;
                break;
            }
            if (string.IsNullOrEmpty(popupWindowHandle)) return result.Failed();
            CurrentBrowser.Driver.SwitchTo().Window(popupWindowHandle);

            return result.Success();
        }
    }
}
