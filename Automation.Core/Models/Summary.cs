using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bromine.Automation.Core.Enum;
using log4net;

namespace Bromine.Automation.Core.Models
{
  public class Summary
  {
    public static readonly ILog SummaryLogger = LogManager.GetLogger(Assembly.GetExecutingAssembly(), "SummaryLog");
    public Dictionary<SummaryFields, string> LogData { get; }

    static Summary()
    {
      SummaryLogger.Info(string.Join(",", System.Enum.GetNames(typeof(SummaryFields))).TrimEnd(','));
    }

    public Summary()
    {
      LogData = System.Enum.GetValues(typeof(SummaryFields)).Cast<SummaryFields>().ToDictionary(k => k, v => string.Empty);
    }

    public void LogSummary()
    {
      LogData[SummaryFields.TimeStamp] = DateTime.Now.ToString("yyyyMMdd_HHMMssff");
      SummaryLogger.Info(string.Join(",", LogData.Values).TrimEnd(','));
    }
  }
}
