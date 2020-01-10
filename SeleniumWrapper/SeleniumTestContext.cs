using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Interfaces;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Helpers;
using Bromine.SeleniumWrapper.Profiles;
using Bromine.SeleniumWrapper.Tasks;
using Bromine.SeleniumWrapper.Tasks.Browser;
using Bromine.SeleniumWrapper.Tasks.ConsoleLog;
using log4net;
using OpenQA.Selenium;

namespace Bromine.SeleniumWrapper
{
    public class SeleniumTestContext : ITestContext
    {
        private readonly ILog _logger = LogHelper.GetLogger();
        private readonly Lazy<HttpHelper> _httpHelper;
        private ConsoleLogHelper _consoleLogHelper;

        #region Properties

        //public static string MainWindowHandle = "";
        private static readonly List<SeleniumBrowser> MainBrowsers = new List<SeleniumBrowser>();

        public SeleniumTestContext()
        {
            _httpHelper = new Lazy<HttpHelper>();
        }

        #endregion

        #region ITestContext Implementation

        public Task<TaskResult> Execute(TaskInfo task, TestStorage storage)
        {
            var seleniumTask = CreateTask(task, storage);
            seleniumTask.Storage = storage;
            seleniumTask.HttpHelper = _httpHelper;
            seleniumTask.ConsoleLogHelper = _consoleLogHelper;
            _logger.Debug($"[Browser:{seleniumTask.CurrentBrowser.Id}],[LogLocation:{seleniumTask.CurrentBrowser.ProfileLocation}]");
            return seleniumTask.CheckDriver() ? seleniumTask.Process(task) : new TaskResult(false).Result();
        }

        public void Dispose()
        {
            if (Constants.AppConfig.LeaveBrowserOpenOnExit) return;
            Dispose(MainBrowsers);
        }

        public bool IsCacheSupported => true;

        public bool IsSeleniumUsed => true;

        #endregion

        #region Private Methods

        private void InitializeBrowser(TaskInfo taskInfo, TestStorage storage)
        {
            // Setup browser first time
            var testInfo = taskInfo.Parent;
            if (testInfo.Browser != null) return;

            var loadHomePage = new Action<SeleniumBrowser>((browser) =>
            {
                try
                {
                    browser.Driver.Navigate().GoToUrl(Constants.AppConfig.BaseUrl);
                }
                catch (WebDriverException ex)
                {
                    _logger.Error($"[Browser:{browser.Id}] Navigation failed with error '{ex.Message}' and url '{Constants.AppConfig.BaseUrl}'", ex);
                    testInfo.Browser = browser;
                    throw;
                }
            });

            lock (MainBrowsers)
            {
                SeleniumBrowser browser;
                var availableBrowsers = MainBrowsers
                    .Where(x => x.BrowserType.Equals(testInfo.BrowserType) && x.Language.Equals(testInfo.Language) && !x.IsBusy).ToList();
                var reloadHeaders = false;
                if (availableBrowsers.Any())
                {
                    var sameHeaderBrowser = availableBrowsers.FirstOrDefault(x => (testInfo.BrowserHeader == null && x.HeaderSetup == null) ||
                                                                                  (x.HeaderSetup != null &&
                                                                                   x.HeaderSetup.Equals(testInfo.BrowserHeader)));
                    if (sameHeaderBrowser != null)
                    {
                        // Browser available with same header
                        browser = sameHeaderBrowser;
                        // Sometimes launch url breaks so handling it
                        loadHomePage(browser);
                        _logger.Info($"[Browser:{browser.Id}] Reusable browser instance found with header '{browser.HeaderSetup?.CustomUserAgent}'");
                    }
                    else
                    {
                        // No browsers available with same header so, using any browser
                        browser = availableBrowsers.First();
                        browser.HeaderSetup = testInfo.BrowserHeader;
                        reloadHeaders = true;
                        _logger.Info($"[Browser:{browser.Id}] Reassigning the existing browser with header '{browser.HeaderSetup?.CustomUserAgent}'");
                    }
                }
                else
                {
                    browser = GetNewBrowser(testInfo);
                    reloadHeaders = true;
                }

                if (reloadHeaders)
                {
                    loadHomePage(browser);
                    browser.SetHeader();
                }

                //MainWindowHandle = browser.Driver.CurrentWindowHandle;
                browser.IsBusy = true;
                testInfo.Browser = browser;
                // Create console helper once per test for all tasks
                _consoleLogHelper = new ConsoleLogHelper(storage, browser);
                _consoleLogHelper.Clear();
            }
        }

        public void Dispose(SeleniumBrowser browser)
        {
            lock (MainBrowsers)
            {
                try
                {
                    browser.Driver.Dispose();
                    browser.Driver.Quit();
                    _logger.Info($"[Browser:{browser.Id}] Browser '{browser.HeaderSetup?.CustomUserAgent}' removed!");
                    MainBrowsers.Remove(browser);
                }
                catch (Exception ex)
                {
                    //log and ignore the exception
                    _logger.Info($"Error: {ex.Message}");
                }
            }
        }

        private void Dispose(IReadOnlyList<SeleniumBrowser> browsers)
        {
            for (var index = browsers.Count - 1; index >= 0; index--)
            {
                Dispose(browsers[index]);
            }
        }

        private BrowserTask CreateTask(TaskInfo task, TestStorage storage)
        {
            InitializeBrowser(task, storage);
            BrowserTask seleniumTask;
            switch (task.Action)
            {
                case ActionType.Click:
                    seleniumTask = new ClickTask();
                    break;
                case ActionType.AddCookie:
                    seleniumTask = new AddCookieTask();
                    break;
                case ActionType.DeleteCookie:
                    seleniumTask = new DeleteCookieTask();
                    break;
                case ActionType.Input:
                    seleniumTask = new InputTask();
                    break;
                case ActionType.SelectDropdown:
                    seleniumTask = new SelectDropdownTask();
                    break;
                case ActionType.UlSelectDropdown:
                    seleniumTask = new UlSelectDropdownTask();
                    break;
                case ActionType.Navigate:
                    seleniumTask = new NavigateTask();
                    break;
                case ActionType.SwitchWindow:
                    seleniumTask = new WindowTask();
                    break;
                case ActionType.VerifyUi:
                    seleniumTask = new VerifyUiTask();
                    break;
                case ActionType.VerifyUrl:
                    seleniumTask = new VerifyUrlTask();
                    break;
                case ActionType.Wait:
                    seleniumTask = new WaitTask();
                    break;
                case ActionType.WaitUntil:
                    seleniumTask = new WaitUntilLogTask();
                    break;
                case ActionType.ExecuteJavaScript:
                    seleniumTask = new ExecuteJavaScriptTask();
                    break;
                case ActionType.TakeScreenShot:
                    seleniumTask = new TakeScreenshotTask();
                    break;
                case ActionType.NavigateBack:
                    seleniumTask = new NavigateBackTask();
                    break;
                case ActionType.CloseWindow:
                    seleniumTask = new WindowTask();
                    break;
                case ActionType.VerifyCssProperties:
                    seleniumTask = new VerifyCssPropertiesTask();
                    break;
                case ActionType.VerifyAttributes:
                    seleniumTask = new VerifyAttributesTask();
                    break;
                case ActionType.HandlePrompt:
                    seleniumTask = new HandlePromptTask();
                    break;
                case ActionType.InputEmail:
                    seleniumTask = new InputTask();
                    break;
                case ActionType.VerifyCookie:
                    seleniumTask = new VerifyCookieTask();
                    break;
                case ActionType.ExtractStringFromUrl:
                    seleniumTask = new ExtractStringFromUrlTask();
                    break;
                case ActionType.DecodeUrl:
                    seleniumTask = new DecodeUrlTask();
                    break;
                case ActionType.RewriteUrl:
                    seleniumTask = new RewriteUrlTask();
                    break;
                case ActionType.FetchDataFromUi:
                    seleniumTask = new FetchDataFromUiTask();
                    break;
                case ActionType.Refresh:
                    seleniumTask = new RefreshTask();
                    break;
                case ActionType.AddAuthHeader:
                    seleniumTask = new AddAuthHeaderTask();
                    break;
                case ActionType.MatchLog:
                    seleniumTask = new MatchLogTask();
                    break;
                case ActionType.ReadValueFromLog:
                    seleniumTask = new ReadValueFromLogTask();
                    break;
                case ActionType.SwitchContextToIframe:
                    seleniumTask = new WindowTask();
                    break;
                default:
                    seleniumTask = new BrowserTask();
                    break;
            }

            seleniumTask.CurrentBrowser = task.Parent.Browser;
            return seleniumTask;
        }

        private SeleniumBrowser GetNewBrowser(TestInfo testInfo)
        {
            var browser = WebBrowserFactory.Create(testInfo.BrowserType, testInfo.BrowserHeader?.GetDictionary(), testInfo.Language,
                Thread.CurrentThread.ManagedThreadId);
            browser.HeaderSetup = testInfo.BrowserHeader;
            MainBrowsers.Add(browser);
            _logger.Info(
                $"[Browser:{browser.Id}] created with header '{browser.HeaderSetup?.CustomUserAgent}' with culture language '{browser.Language}' " +
                $"{(_logger.IsDebugEnabled ? $" and log Location='{browser.ProfileLocation}'" : string.Empty)}");
            return browser;
        }

        #endregion
    }
}