using System;
using System.Linq;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Models;
using Bromine.SeleniumWrapper.Helpers;

namespace Bromine.SeleniumWrapper
{
  public class BrowserTask : BaseTask
  {
    public SeleniumBrowser CurrentBrowser;
    public Lazy<HttpHelper> HttpHelper { get; set; }
    public ConsoleLogHelper ConsoleLogHelper { get; set; }

    protected void Info(string message)
    {
      Logger.Info($"[Browser:{CurrentBrowser?.Id}] {message}");
    }

    protected void Debug(string message)
    {
      Logger.Debug($"[Browser:{CurrentBrowser?.Id}] {message}");
    }

    protected void Error(string message)
    {
      Logger.Error($"[Browser:{CurrentBrowser?.Id}] {message}");
    }

    protected void Warn(string message)
    {
      Logger.Warn($"[Browser:{CurrentBrowser?.Id}] {message}");
    }

    public bool CheckDriver()
    {
      try
      {
        return CurrentBrowser?.Driver != null && CurrentBrowser.Driver.WindowHandles.Any();
      }
      catch (Exception ex)
      {
        Error($"Browser Crashed:{ex.Message}\nStackTrace: {ex.StackTrace}");
        throw new BrowserException("Browser Crashed");
      }
    }
  }
}
