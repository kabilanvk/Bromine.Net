using System.Collections.Generic;
using Bromine.Automation.Core.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bromine.Automation.Core.Models
{
    public class TestInfo
    {
        public TestInfo()
        {
            Tasks = new List<TaskInfo>();
        }

        #region Json Properties

        public string Name { get; set; }
        public string Header { get; set; }
        public string Region { get; set; }
        public string Plugin { get; set; }
        public string Language { get; set; }

        #endregion

        #region Internal Properties

        public string FullName { get; set; }
        public List<string> Category { get; set; }
        // Browser type defaults to FF for now
        public BrowserType BrowserType { get; set; }
        // Selenium driver used for test run
        public SeleniumBrowser Browser { get; set; }
        public EnvironmentType Environment { get; set; }
        public BrowserHeader BrowserHeader { get; set; }
        public List<TaskInfo> Tasks { get; }
        public TestContext Context { get; set; }

        #endregion
    }
}
