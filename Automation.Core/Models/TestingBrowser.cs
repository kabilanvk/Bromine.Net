using System.Diagnostics;
using Bromine.Automation.Core.Enum;

namespace Bromine.Automation.Core.Models
{
    public class TestingBrowser
    {
        public BrowserType Type { get; set; }
        public FileVersionInfo Version { get; set; }
    }
}
