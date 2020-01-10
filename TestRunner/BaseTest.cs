using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Models;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Bromine.TestRunner
{
    [TestClass]
    public class BaseTest
    {
        private readonly ILog _log = LogHelper.GetDefaultLogger();

        public TestContext TestContext { get; set; }

        #region Test Execution Helper

        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        {
            TestRunner.Initialize(testContext);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            TestRunner.CleanUp();
        }

        protected async Task RunTest(string testInfo, [CallerMemberName] string caller = null)
        {
            await RunTestInternal(JsonConvert.DeserializeObject<TestInfo>(testInfo), caller);
        }

        protected async Task RunTest(TestInfo testInfo, [CallerMemberName] string caller = null)
        {
            await RunTestInternal(testInfo, caller);
        }

        #endregion

        #region Private Helpers

        private async Task RunTestInternal(TestInfo testInfo, string caller)
        {
            var testMethod = GetType().GetMethod(caller);
            var testCategories = testMethod?.GetCustomAttributes<TestCategoryAttribute>().ToList();
            testInfo.Category = testCategories?.SelectMany(c => c.TestCategories).ToList();
            testInfo.Name = testMethod?.Name;
            testInfo.FullName = $"{testMethod?.DeclaringType?.FullName}";
            testInfo.Context = TestContext;
            Initialize(testInfo);
            await TestRunner.Runner(testInfo);
        }

        private void Initialize(TestInfo testInfo)
        {
            // Defaulting browser to FF
            testInfo.BrowserType = BrowserType.FireFox;
            // Filter test case based on environment
            if (!SetEnvironment(testInfo)) Assert.Inconclusive("No test data to execute");
            testInfo.Language = string.IsNullOrEmpty(testInfo.Language) ? Constants.DefaultLanguage.ToLower() : testInfo.Language.ToLower();
            // Set the custom user agent header
            SetHeader(testInfo);
        }

        private static bool SetEnvironment(TestInfo testInfo)
        {
            // GetEnv name, Env, if exists, should have this format: INT2/INT/BETA/PROD, not case-senstive
            // Do NOT Default to COS-INT to not load test cases from other environments
            var envType = testInfo.Category.Any(env => env.Equals(Constants.AppConfig.RuntimeEnvironment.ToString(),
                StringComparison.OrdinalIgnoreCase))
                ? Constants.AppConfig.RuntimeEnvironment
                : EnvironmentType.Null;

            testInfo.Environment = envType;
            return (EnvironmentType.Null != envType);
        }

        private void SetHeader(TestInfo testInfo)
        {
            // testcase.Parent = this;
            var browserHeader = new BrowserHeader();
            // Useragent (user-agent and x-symc-user-agent) settings
            if (testInfo.Header == null) return;
            lock (TestCore.TestConfiguration)
            {
                if (!TestCore.TestConfiguration.HeaderConfiguration.HeaderSets.Exists(item => item.Key.Equals(testInfo.Header, StringComparison.OrdinalIgnoreCase)))
                {
                    _log.Error($"User agent {testInfo.Header} in meta data is not found in HeaderConfiguration");
                    throw new Exception($"User agent {testInfo.Header} in meta data is not found in HeaderConfiguration");
                }
                var headerSet = TestCore.TestConfiguration.HeaderConfiguration.HeaderSets.First(item => item.Key == testInfo.Header);
                foreach (var headerProperty in TestCore.TestConfiguration.HeaderConfiguration.HeaderProperties)
                {
                    // If particular header (e.g. user-agent, x-symc-user-agent) is present in the headerset specificed in test case
                    if (headerSet.GetType().GetProperty(headerProperty.Key) == null) continue;

                    // If the header is auto loaded but included in the excluded header list, don't do anything
                    if (headerSet.ExcludedHeaders != null && headerSet.ExcludedHeaders.Contains(headerProperty.Key)) continue;

                    // If (excludedHeadersPI != null) continue;
                    // Add to browserheader auto loaded and default from app.config, and set the value with the same key in browser header
                    var headerSetProperty = typeof(HeaderSet).GetProperty(headerProperty.Key);
                    var valueinHeaderSet = headerSetProperty?.GetValue(headerSet, null);
                    if (headerProperty.AutoLoaded && (valueinHeaderSet == null) &&
                        typeof(Constants.AppConfig).GetProperty(headerProperty.DefaultAppConfigKey) != null)
                    {
                        var autoLoadedProperty = typeof(Constants.AppConfig).GetProperty(headerProperty.DefaultAppConfigKey);
                        var autoLoadedValue = autoLoadedProperty?.GetValue(typeof(Constants.AppConfig), null);
                        if (autoLoadedValue == null) continue;
                        Utilities.SetPropertyValue(headerProperty.Key, autoLoadedValue, browserHeader);
                    }
                    else if (valueinHeaderSet != null)
                    {
                        // Get the value of the key from specific header set, and set the value with the same key in browser header
                        Utilities.SetPropertyValue(headerProperty.Key, valueinHeaderSet, browserHeader);
                    }
                }
                // Region settings
                if (testInfo.Region != null)
                {
                    if (!TestCore.TestConfiguration.RegionConfiguration.Exists(item => item.Key.Equals(testInfo.Region, StringComparison.OrdinalIgnoreCase)))
                    {
                        _log.Error($"Region {testInfo.Region} in meta data is not found in configuration");
                        throw new Exception($"Region {testInfo.Region} in meta data is not found in configuration");
                    }
                    var ipAddress = TestCore.TestConfiguration.RegionConfiguration.First(item =>
                      item.Key.Equals(testInfo.Region, StringComparison.OrdinalIgnoreCase)).Ip;
                    browserHeader.XForwardedFor = ipAddress;
                }
                // Adding XForwardedFor for local
                if (string.IsNullOrEmpty(browserHeader.XForwardedFor) && Constants.AppConfig.BuildEnvironment == EnvironmentType.Local)
                {
                    browserHeader.XForwardedFor = "127.0.0.1";
                }
            }
            testInfo.BrowserHeader = browserHeader;
        }

       

        #endregion
    }
}