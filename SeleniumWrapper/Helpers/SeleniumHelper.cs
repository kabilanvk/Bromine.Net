using System;
using System.Collections.Generic;
using System.Linq;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Models;
using OpenQA.Selenium;

namespace Bromine.SeleniumWrapper.Helpers
{
  public static class SeleniumHelper
  {
    private const string ScriptGetLog = "var callback=arguments[arguments.length-1];ext.exec({cmd:\"getlog\"}).then(function(e){callback(e);});";
    private const string ScriptClear = "var callback=arguments[arguments.length-1];ext.exec({cmd:\"clearlog\"}).then(ext.exec({cmd:\"clearcookies\"}).then(function(e){callback(e);}));";
    private const string ScriptHeader = "var callback=arguments[arguments.length-1];ext.exec({{cmd:\"setheader\",data:\"{0}\"}}).then(function(e){{callback(e);}});";

    public static By LocateBy(this LocateByType lookupType, string lookupValue)
    {
      switch (lookupType)
      {
        case LocateByType.Id:
          return By.Id(lookupValue);
        case LocateByType.Css:
          return By.CssSelector(lookupValue);
        case LocateByType.Class:
          return By.ClassName(lookupValue);
        case LocateByType.LinkText:
          return By.LinkText(lookupValue);
        case LocateByType.Name:
          return By.Name(lookupValue);
        case LocateByType.PartialLinkText:
          return By.PartialLinkText(lookupValue);
        case LocateByType.Xpath:
          return By.XPath(lookupValue);
        default:
          return By.TagName(lookupValue);
      }
    }

    public static void CloseOtherWindows(IWebDriver driver, string windowHandleToKeep)
    {
      var windowHandles = driver.WindowHandles;
      foreach (var handle in windowHandles)
      {
        if (string.Compare(handle, windowHandleToKeep, StringComparison.OrdinalIgnoreCase) != 0)
        {
          driver.SwitchTo().Window(handle);
          driver.Close();
        }
      }

      driver.SwitchTo().Window(windowHandleToKeep); //switch back to main window
    }

    public static void Cleanup(SeleniumBrowser browser, string language)
    {
      if (browser?.Driver == null) return;
      var log = LogHelper.GetLogger();
      log.Debug("Starting Selenium cleanup work...");
      // Only need to clean up the extra windows in the currently driver. Other drivers will be handled in their own test cases
      try
      {
        CloseOtherWindows(browser.Driver, browser.CurrentWindow);
        //for performace, close a window if it is non-English
        if (!Constants.DefaultLanguage.Equals(language, StringComparison.OrdinalIgnoreCase))
        {
          browser.Driver.SwitchTo().Window(browser.CurrentWindow);
          browser.Driver.Close();
        }
      }
      catch (Exception ex)
      {
        log.Error(ex);
      }
      log.Debug("All non-main windows have been closed & cleanup completed");
      // Make browser available for next test run
      browser.IsBusy = false;
      log.Info($"[Browser:{browser.Id}] Browser {browser.HeaderSetup?.CustomUserAgent} freedup for the next test case.");
    }

    #region Firefox Extension Helper Functions

    public static string GetHeadersJson(Dictionary<string, string> headers)
    {
      var headerList = headers.Select(h => $"{{'key':'{h.Key}','val':'{h.Value}'}}").ToList();
      return $"[{string.Join(",", headerList)}]";
    }

    public static string GetConsoleLogs(this SeleniumBrowser browser)
    {
      return ((IJavaScriptExecutor)browser.Driver).ExecuteAsyncScript(ScriptGetLog).ToString();
    }

    public static string ClearBrowserData(this SeleniumBrowser browser)
    {
      return ((IJavaScriptExecutor)browser.Driver).ExecuteAsyncScript(ScriptClear).ToString();
    }

    public static string SetHeader(this SeleniumBrowser browser)
    {
      if (browser.HeaderSetup == null) return null;
      var headers = GetHeadersJson(browser.HeaderSetup.GetDictionary());
      return ((IJavaScriptExecutor)browser.Driver).ExecuteAsyncScript(string.Format(ScriptHeader, headers)).ToString();
    }

    #endregion
  }
}
