using System.Collections.Generic;
using System.IO;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Models;
using Newtonsoft.Json;

namespace Bromine.Automation.Core.Helpers
{
    public static class TestCore
    {
        #region Properties

        public static TestConfiguration TestConfiguration { get; set; }

        public static Dictionary<string, string> Resources { get; set; }

        // Store the browser setting in particular machine to avoid fetching the setting for each test case when loaded on the same machine
        public static Dictionary<string, TestingBrowser> MachineBrowsers { get; set; }

        #endregion

        #region Constructor

        static TestCore()
        {
            Resources = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Constants.AppConfig.TestResourcesJsonFileName));
            MachineBrowsers = new Dictionary<string, TestingBrowser>();
            TestConfiguration = JsonConvert.DeserializeObject<TestConfiguration>(File.ReadAllText(Constants.AppConfig.TestConfigurationJsonPath));
        }

        #endregion
    }
}
