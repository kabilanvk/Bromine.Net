using System;
using System.Collections.Generic;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Bromine.SeleniumWrapper.Profiles
{
    public static class WebBrowserFactory
    {
        public static SeleniumBrowser Create(BrowserType browserType, Dictionary<string, string> headers, string language, int id)
        {
            IWebDriver driver;
            string tempProfileLocation;

            switch (browserType)
            {
                case BrowserType.Unknown:
                case BrowserType.FireFox:
                    var firefoxOptions = new FirefoxOptions
                    {
                        Profile = new FireFoxProfile(headers, language),
                        BrowserExecutableLocation = Constants.AppConfig.FirefoxBinaryPath,
                        LogLevel = FirefoxDriverLogLevel.Error,
                        AcceptInsecureCertificates = true,
                    };
                    // Opens the firefox browser console 
                    firefoxOptions.AddArgument("-devtools");
                    if (Constants.AppConfig.BrowserHeadlessMode) firefoxOptions.AddArguments("--headless");
                    var firefoxDriverService = FirefoxDriverSetup.SetupFirefoxDriverService();
                    //driver = new FirefoxDriver(firefoxDriverService, firefoxOptions, TimeSpan.FromMinutes(3));

                    // Hack until selenium fixes the firefox driver for .Net core
                    driver = new FirefoxDriver(new Uri($"http://127.0.0.1:{firefoxDriverService.Port}"), firefoxOptions);
                    // End of hack
                    ((FirefoxDriver)driver).InstallWebExtension(Constants.AppConfig.FfExConsoleListenerPath);
                    tempProfileLocation = firefoxOptions.Profile.ProfileDirectory;
                    driver.Manage().Window.Maximize();
                    driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(20);
                    break;
                default:
                    throw new Exception("Not able to start the testing browser, check app.config settings.");
            }
            if (driver == null)
            {
                throw new WebDriverException("CurrentBrowser not initialized!");
            }
            return new SeleniumBrowser(id)
            {
                Driver = driver,
                CurrentWindow = driver.CurrentWindowHandle,
                BrowserType = browserType,
                ProfileLocation = tempProfileLocation,
                ConsoleLogLocation = System.IO.Path.Combine(tempProfileLocation, Constants.ConsoleLogFileName),
                HeaderConfigLocation = System.IO.Path.Combine(tempProfileLocation, Constants.HeadersConfigFileName),
                Language = language
            };
        }
    }
}

