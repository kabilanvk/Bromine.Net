using System;
using System.Threading;
using System.Web;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace Bromine.SeleniumWrapper.Helpers
{
  public static class WebDriverExtension
  {
    private static int _waitTimeout = 2000;

    public static IWebElement WaitAndGetElement(this IWebDriver driver, By by)
    {
      return GetElement(driver, by, true);
    }

    public static bool IsElementFoundAfterWait(this IWebDriver driver, By by)
    {
      //driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
      //driver.FindElement(by);
      return GetElement(driver, by, true) != null;
    }

    public static bool IsElementFoundAfterWait(this IWebDriver driver, string text)
    {
      return GetElement(driver, By.XPath($"//*[text()[contains(.,\"{text}\")]]"), false) != null;
    }

    public static bool IsElementFound(this IWebDriver driver, By element)
    {
      int countOfElements = driver.FindElements(element).Count;
      if (countOfElements == 0)
      {
        return true;
      }
      else
      {
        return (false == driver.FindElement(element).Displayed);
      }
    }

    public static bool IsElementFound(this IWebDriver driver, string text)
    {
      int countOfElements = driver.FindElements(By.XPath($"//*[contains(text(),\"{text}\")]")).Count;
      if (countOfElements == 0)
      {
        return true;
      }
      else
      {
        return (false == driver.FindElement(By.XPath($"//*[contains(text(),\"{text}\")]")).Displayed);
      }
    }

    public static bool WaitForModal(this IWebDriver webDriver)
    {
      var logger = LogHelper.GetLogger();
      try
      {
        var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(Constants.AppConfig.SeleniumFindElementWaitTime));
        return wait.Until(drv => WaitForModalPrompt(webDriver));
      }
      catch (WebDriverTimeoutException)
      {
        logger.Info($"Timedout while waiting for prompt after {Constants.AppConfig.SeleniumFindElementWaitTime}secs!");
      }
      return false;
    }

    public static string GetAttributeValueFromElement(this IWebDriver driver, By by, string attribute)
    {
      return GetElement(driver, by, false)?.GetAttribute(attribute);
    }

    public static string GetCssProperties(this IWebDriver driver, By by, string property)
    {
      return GetElement(driver, by, false)?.GetCssValue(property);
    }

    public static void WaitForPageLoad(this IWebDriver driver, bool isExceptionSuppressed = false)
    {
      var logger = LogHelper.GetLogger();
      try
      {
        new WebDriverWait(driver, TimeSpan.FromSeconds(Constants.AppConfig.SeleniumFindElementWaitTime)).Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
      }
      catch (Exception ex)
      {
        if (isExceptionSuppressed)
        {
          logger.Debug($"Suppressed exception in WaitForPageLoad: {ex.Message}");
        }
        else
        {
          throw new Exception($"Exception in WaitForPageLoad: {ex.Message}");
        }
      }
    }

    public static void SendKeys(this IWebDriver driver, string keyCombination)
    {
      GetElement(driver, By.TagName("body"), false).SendKeys(keyCombination);
    }

    public static bool PerformClick(this IWebDriver driver, By by)
    {
      var logger = LogHelper.GetLogger();
      var element = driver.GetElement(by, true);
      if (element == null || !element.Enabled)
      {
        logger.Info("Unable to perform click because element either not found or disabled.");
        return false;
      }
      // Until we figureout why anchor click spans a new tab with href link
      var hrefVal = element.GetAttribute("href");

      if (!string.IsNullOrEmpty(hrefVal) && (hrefVal.Equals("javascript:;", StringComparison.InvariantCultureIgnoreCase) ||
                                             hrefVal.Equals("#", StringComparison.InvariantCultureIgnoreCase)))
      {
        logger.Info("Element found with empty href link so, removing attribute to avoid new tab popup issue");
        driver.ExecuteJavaScript("arguments[0].removeAttribute('href');", element);
        hrefVal = string.Empty;
      }

      try
      {
        if (string.IsNullOrEmpty(hrefVal))
        {
          //No href or href with empty javascript - use Click
          element.Click();
        }
        else
        {
          //regular anchor click with href - use SendKeys
          logger.Info("Element found with href link so sendkeys to avoid new popup");
          element.SendKeys(Keys.Enter);
        }
        // Adding a wait after redirect to resolve page lookup and logs not found issue
        Thread.Sleep(2000);
      }
      catch (WebDriverException ex)
      {
        logger.Info($"Click failed with {ex.Message}. Waiting for 2 secs before retrying");
        Thread.Sleep(2000);
        element.Click();
      }
      return true;
    }

    public static bool WaitForUrl(this IWebDriver driver, string url)
    {
      var logger = LogHelper.GetLogger();
      try
      {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Constants.AppConfig.SeleniumFindElementWaitTime));
        return wait.Until(drv =>
        {
          if (VerifyUrl(driver, url)) return true;
          Thread.Sleep(_waitTimeout);
          return false;
        });
      }
      catch (WebDriverTimeoutException)
      {
        logger.Info($"Timedout while waiting for url '{url}' after {Constants.AppConfig.SeleniumFindElementWaitTime}secs!");
      }
      return false;
    }

    public static bool VerifyUrl(this IWebDriver driver, string url)
    {
            var currentUrl = driver.Url;
            if (currentUrl.IndexOf(url, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            
            //.net core encode anything added to URL.
            var decodedUrl = HttpUtility.UrlDecode(currentUrl);
            return decodedUrl.IndexOf(url, StringComparison.OrdinalIgnoreCase) >= 0;
        }

    #region Private Methods

    private static IWebElement GetElement(ISearchContext driver, By by)
    {
      if (driver == null) return null;
      IWebElement element = null;
      try
      {
        element = driver.FindElement(by);
      }
      catch (NoSuchElementException)
      {
        Thread.Sleep(_waitTimeout);
      }
      return element;
    }

    private static IWebElement GetDisplayedElement(ISearchContext driver, By by)
    {
      var element = GetElement(driver, by);
      return element != null && element.Displayed ? element : null;
    }

    public static IWebElement GetElement(this IWebDriver driver, By by, bool displayCheck)
    {
      var logger = LogHelper.GetLogger();
      IWebElement element = null;
      try
      {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Constants.AppConfig.SeleniumFindElementWaitTime));
        element = wait.Until(drv => displayCheck ? GetDisplayedElement(driver, by) : GetElement(driver, by));
      }
      catch (WebDriverTimeoutException)
      {
        logger.Info($"Timeout occurred while waiting for element '{by}'");
      }
      return element;
      //return wait.Until(drv => drv.FindElement(by));
    }

    private static bool WaitForModalPrompt(this IWebDriver webDriver)
    {
      try
      {
        webDriver.SwitchTo().Alert();
        return true;
      }
      catch (NoAlertPresentException)
      {
        //Wait for a second before retry
        Thread.Sleep(_waitTimeout);
      }

      return false;
    }

    #endregion
  }
}
