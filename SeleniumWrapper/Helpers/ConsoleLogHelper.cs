using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Models;
using log4net;

namespace Bromine.SeleniumWrapper.Helpers
{
  public class ConsoleLogHelper
  {
    private readonly ILog _logger = LogHelper.GetLogger();
    private readonly SeleniumBrowser _browser;
    private readonly TestStorage _storage;

    public ConsoleLogHelper(TestStorage storage, SeleniumBrowser browser)
    {
      _storage = storage;
      _browser = browser;
    }

    #region Public Members

    public void Clear()
    {
      ClearConsoleListenerLogs();
      _logger.Debug($"[Browser:{_browser.Id}] Console log cleared. Header='{_browser.HeaderSetup?.CustomUserAgent}'");
      _storage.Cache.Remove(Constants.Log);
    }

    public bool PerformMatch(IEnumerable<string> findStringList)
    {
      var data = GetConsoleLog();
      if (string.IsNullOrEmpty(data)) return false;
      var currentPos = 0;
      var lastmatch = string.Empty;
      foreach (var findString in findStringList)
      {
        var match = data.IndexOf(findString, currentPos + 1, StringComparison.OrdinalIgnoreCase);
        if (match < 0)
        {
          _logger.Info($"[Browser:{_browser.Id}] No match found for the requested '{findString}' " +
                        $"{(string.IsNullOrEmpty(lastmatch) ? "" : $", Last match='{lastmatch}'")}, " +
                        $"Cursor position={currentPos}");
          return false;
        }
        currentPos = match;
        lastmatch = findString;
      }
      return true;
    }

    public bool PerformMatchWithoutOrder(IEnumerable<string> findStringList)
    {
      var data = GetConsoleLog();
      if (string.IsNullOrEmpty(data)) return false;
      var lastmatch = string.Empty;
      foreach (var findString in findStringList)
      {
        var match = data.IndexOf(findString, StringComparison.OrdinalIgnoreCase);
        if (match < 0)
        {
          _logger.Info($"[Browser:{_browser.Id}] No match found for the requested '{findString}' " +
                       $"{(string.IsNullOrEmpty(lastmatch) ? "" : $", Last match='{lastmatch}'")}");
          return false;
        }
        lastmatch = findString;
      }
      return true;
    }

    public bool PerformRegexMatch(IEnumerable<string> regexList, bool negated)
    {
      var data = GetConsoleLog();
      if (string.IsNullOrEmpty(data)) return false;
      foreach (var exp in regexList)
      {
        _logger.Info($"Checking regex '{exp}'...");
        var match = Regex.Match(data, exp, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        var result = match.Success ^ negated;
        if (!result)
        {
          _logger.Info($"Match {(!negated ? "not " : string.Empty)}found for exp '{exp}' with value '{match.Value}'");
          return false;
        }
        _logger.Info($"Match {(negated ? "not " : string.Empty)}found for exp '{exp}' with value '{match.Value}'");
      }
      return true;
    }

    public string GetConsoleLog(bool forceFetch = false, bool status = true)
    {
      if (_storage.Cache.Contains(Constants.Log) && !forceFetch) return _storage.Cache.Get(Constants.Log);
      var logs = GetConsoleListenerLogs();
      var logKey = $"Location='{_browser.ConsoleLogLocation}'";

      if (string.IsNullOrEmpty(logs))
      {
        if (status) _logger.Info($"[Browser:{_browser.Id}] No logs available from {logKey}");
        return string.Empty;
      }
      _storage.Cache.AddOrUpdate(Constants.Log, logs);
      return _storage.Cache.Get(Constants.Log);
    }

    #endregion

    #region Private Members

    private string GetConsoleListenerLogs()
    {
      return _browser.GetConsoleLogs();
    }

    private void ClearConsoleListenerLogs()
    {
      _browser.ClearBrowserData();
    }
    #endregion
  }
}
