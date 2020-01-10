using System.Collections.Generic;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Helpers;

namespace Bromine.Automation.Core.Common
{
    public static class Constants
    {
        public const string HeaderAccept = "accept";
        public const string HeaderContentType = "content-type";
        public const string HeaderAuthorization = "authorization";
        public const string HeaderHost = "host";
        public const string ContentTypeJson = "application/json";
        public const string JsReturnValue = "@JSParam";
        public const string Log = "@Log";
        public const string DefaultLanguage = "en-US";
        public const string LogFileName = "LogName";
        public const string DefaultLogFileName = "UIAutomation.log";
        public const string UserAgent = "UserAgent";
        public const string ConsoleLogFileName = "Console.log";
        public const string HeadersConfigFileName = "Headers.config";

        public static class AppConfig
        {
            static AppConfig()
            {
                KnownUrls.Add("BaseUrl", BaseUrl);
            }

            public static Dictionary<string, string> KnownUrls => ConfigurationHelper.GetSection<Dictionary<string,string>>("appSettings:KnownUrls");
            public static string BaseUrl => ConfigurationHelper.GetConfig("BaseUrl");
            public static string TestConfigurationJsonPath => ConfigurationHelper.GetConfig("TestConfigurationJsonPath");
            public static string TestDataBasePath => ConfigurationHelper.GetConfig("TestDataBasePath");
            public static string FfExConsoleListenerPath => ConfigurationHelper.GetConfig("ConsoleListenerPath");
            private static EnvironmentType _env;
            public static EnvironmentType RuntimeEnvironment => (System.Enum.TryParse(ConfigurationHelper.GetConfig("RuntimeEnvironment"), out _env))
                ? _env
                : EnvironmentType.Null;
            public static EnvironmentType BuildEnvironment => (System.Enum.TryParse(ConfigurationHelper.GetConfig("BuildEnvironment"), out _env))
                ? _env
                : EnvironmentType.Null;
            public static bool IsScreenshotOn => ConfigurationHelper.GetConfig<bool>("IsScreenshotOn");
            public static string TestResourcesJsonFileName => ConfigurationHelper.GetConfig("TestResourcesJson");
            public static int SeleniumFindElementWaitTime => ConfigurationHelper.GetConfig<int>("SeleniumFindElementWaitTime");
            public static int RetryCount => ConfigurationHelper.GetConfig<int>("RetryCount");
            public static string GeckoDriverDir => ConfigurationHelper.GetConfig("GeckoDriverDir");
            public static string FirefoxBinaryPath => ConfigurationHelper.GetConfig("FirefoxBinaryPath");
            public static string Log4NetConfigLocation => ConfigurationHelper.GetConfig("Log4NetConfigLocation");
            public static bool LeaveBrowserOpenOnExit => ConfigurationHelper.GetConfig<bool>("LeaveBrowserOpenOnExit");
            public static bool BrowserHeadlessMode => ConfigurationHelper.GetConfig<bool>("BrowserHeadlessMode");
        }
    }
}

