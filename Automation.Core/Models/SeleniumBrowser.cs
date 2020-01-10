using Bromine.Automation.Core.Enum;
using OpenQA.Selenium;

namespace Bromine.Automation.Core.Models
{
  public class SeleniumBrowser
  {
    public SeleniumBrowser(int id)
    {
      Id = id;
    }

    public IWebDriver Driver { get; set; }
    // Guid to identify current open window
    public string CurrentWindow { get; set; }
    public BrowserHeader HeaderSetup { get; set; }
    // Multi-browser support
    public BrowserType BrowserType { get; set; }
    public string ProfileLocation { get; set; }
    public string ConsoleLogLocation { get; set; }
    public string HeaderConfigLocation { get; set; }
    // To identify whether its currently used by any test run or not
    public bool IsBusy { get; set; }
    public int Id { get; }
    public string Language { get; set; }
  }
}
