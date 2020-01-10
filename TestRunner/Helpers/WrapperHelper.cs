using System;
using System.Collections.Generic;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Interfaces;
using Bromine.Automation.Core.Models;
using Bromine.GenericWrapper;
using Bromine.RestServiceWrapper;
using Bromine.SeleniumWrapper;
using Bromine.TestRunner.Common;
using Bromine.TestRunner.Models;

namespace Bromine.TestRunner.Helpers
{
    public class WrapperHelper
    {
        private Dictionary<WrapperType, ITestContext> _wrappers = new Dictionary<WrapperType, ITestContext>();

        public ITestContext GetTestContext(TaskInfo taskInfo, TestExecutionContext execContext)
        {
            return GetTestContext(GetWrapper(taskInfo), execContext);
        }

        public void ResetTestContext()
        {
            //Close Selenium windows
            foreach (var wrapper in _wrappers.Values)
            {
                //quit Selenium
                wrapper.Dispose();
            }
            //reset wrapper dictionary to load new set at the end of the test cycle
            _wrappers.Clear();
            _wrappers = null;
        }

        private ITestContext GetTestContext(WrapperType adapterType, TestExecutionContext execContext)
        {
            if (_wrappers.ContainsKey(adapterType))
            {
                return _wrappers[adapterType];
            }

            ITestContext testContext;
            lock (_wrappers)
            {
                switch (adapterType)
                {
                    case WrapperType.RestServiceWrapper:
                        testContext = new RestServiceTestContext();
                        break;
                    case WrapperType.GenericWrapper:
                        testContext = new GenericTestContext();
                        break;
                    default:
                        testContext = new SeleniumTestContext();
                        break;
                }
                _wrappers.Add(adapterType, testContext);
            }
            return testContext;
        }

        private static WrapperType GetWrapper(TaskInfo taskInfo)
        {
            switch (taskInfo.Action)
            {
                case ActionType.SilentRequest:
                    return WrapperType.RestServiceWrapper;

                case ActionType.CompareValue:
                case ActionType.GetValueFromCache:
                case ActionType.ClearCacheValue:
                case ActionType.AddTemplate:
                    return WrapperType.GenericWrapper;

                case ActionType.Click:
                case ActionType.Navigate:
                case ActionType.NavigateBack:
                case ActionType.VerifyUi:
                case ActionType.VerifyUrl:
                case ActionType.Input:
                case ActionType.SelectDropdown:
                case ActionType.UlSelectDropdown:
                case ActionType.AddCookie:
                case ActionType.DeleteCookie:
                case ActionType.SwitchWindow:
                case ActionType.Wait:
                case ActionType.WaitUntil:
                case ActionType.ExecuteJavaScript:
                case ActionType.TakeScreenShot:
                case ActionType.CloseWindow:
                case ActionType.VerifyCssProperties:
                case ActionType.VerifyAttributes:
                case ActionType.HandlePrompt:
                case ActionType.InputEmail:
                case ActionType.VerifyCookie:
                case ActionType.ExtractStringFromUrl:
                case ActionType.DecodeUrl:
                case ActionType.RewriteUrl:
                case ActionType.FetchDataFromUi:
                case ActionType.Refresh:
                case ActionType.MatchByRegex:
                case ActionType.MatchLog:
                case ActionType.ReadValueFromLog:
                case ActionType.AddAuthHeader:
                case ActionType.SwitchContextToIframe:
                    return WrapperType.SeleniumWrapper;

                default:
                    throw new NotImplementedException($"ActionType:'{taskInfo.Action}'");
            }
        }
    }
}
