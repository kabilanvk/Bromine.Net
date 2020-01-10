using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Models;
using log4net;
using log4net.Appender;

namespace Bromine.Automation.Core.Helpers
{
    public static class LogHelper
    {
        private static readonly string ConfigLocation = Path.Combine(Environment.CurrentDirectory, Constants.AppConfig.Log4NetConfigLocation);
        public static string LogsBasePath { get; }
        public static AsyncLocal<string> AsyncContext = new AsyncLocal<string>();
        static LogHelper()
        {
            ThreadContext.Properties[Constants.LogFileName] = Constants.DefaultLogFileName;
            AsyncContext.Value = Constants.DefaultLogFileName;
            var logRepo = LogManager.GetRepository(Assembly.GetEntryAssembly());
            log4net.Config.XmlConfigurator.Configure(logRepo, new FileInfo(ConfigLocation));
            LogsBasePath = Path.GetDirectoryName(((FileAppender)LogManager.GetLogger(logRepo.Name, "SummaryLog").Logger.Repository.GetAppenders().First()).File);
        }

        public static ILog GetLogger()
        {
            var logName = ThreadContext.Properties[Constants.LogFileName] ?? AsyncContext.Value;
            if (logName == null || logName.ToString().Equals(Constants.DefaultLogFileName)) return GetDefaultLogger();
            var repositoryName = GetRepository(logName.ToString());
            var log = LogManager.GetLogger(repositoryName, MethodBase.GetCurrentMethod().DeclaringType);
            return log;
        }

        public static ILog GetDefaultLogger()
        {
            var log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            return log;
        }

        #region Test Logger

        public static void SetLogger(TestInfo testInfo)
        {
            SetLogger(testInfo.Name + ".log");
        }

        public static void SetLogger(string logFileName)
        {
            var logName = $"{logFileName}";
            ThreadContext.Properties[Constants.LogFileName] = logName;
            AsyncContext.Value = logName;
            var repo = GetRepository(logName);
            if (LogManager.GetAllRepositories().Any(l => l.Name.Equals(repo))) return;
            var loggerRepository = LogManager.CreateRepository(repo);
            log4net.Config.XmlConfigurator.Configure(loggerRepository, new FileInfo(ConfigLocation));
        }

        #endregion

        private static string GetRepository(string logName)
        {
            return logName + "Repository";
        }
    }
}
