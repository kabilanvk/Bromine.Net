using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Models;
using Bromine.GenericWrapper.Models;
using Bromine.SeleniumWrapper.Helpers;
using Bromine.TestRunner.Models;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TestResult = Bromine.Automation.Core.Models.TestResult;

namespace Bromine.TestRunner.Helpers
{
    public class TestHelper
    {
        #region Variable Declaration

        private readonly ILog _log = LogHelper.GetLogger();
        private readonly TestExecutionContext _execContext;

        public TestHelper(TestExecutionContext execContext)
        {
            _execContext = execContext;
        }

        #endregion

        public async Task<TestResult> ExecuteTest(TestInfo testInfo)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var testResult = new TestResult { Success = true };
            var summary = _execContext.Storage.Summary;
            // don't log sub flows as the TestName changes
            _log.Info($"------- {testInfo.Name} started... -------");
            summary.LogData[SummaryFields.TestName] = testInfo.Name;
            LoadTasks(testInfo);
            summary.LogData[SummaryFields.TaskCount] = testInfo.Tasks.Count.ToString();
            try
            {
                foreach (var taskInfo in testInfo.Tasks)
                {
                    var taskResult = await new TaskHelper(_execContext).ExecuteTask(testInfo, taskInfo);
                    if (!taskResult.Success)
                    {
                        _log.Info($"{taskInfo.TaskName} ({taskInfo.Action}) failed!");
                        summary.LogData[SummaryFields.FailedTask] = $"{taskInfo.TaskName} ({taskInfo.Action})";
                        // Saving failed test log for debugging purpose
                        PersistLog(testInfo);
                        testResult.Success = false;
                        // Retry when there is a recoverable error
                        testResult.Retry = taskResult.IsError;
                        break;
                    }
                    _log.Info($"{taskInfo.TaskName} ({taskInfo.Action}) succeeded.");
                }
            }
            finally
            {
                testResult.Status = $"{testInfo.Name} {(testResult.Success ? "passed" : "failed")}!";
                summary.LogData[SummaryFields.FullName] = testInfo.FullName;
                summary.LogData[SummaryFields.Environment] = testInfo.Environment.ToString();
                summary.LogData[SummaryFields.Plugin] = testInfo.GetPlugin();
                summary.LogData[SummaryFields.TestResult] = testResult.Success ? "Pass" : "Fail";
                summary.LogData[SummaryFields.ExecutionTime] = stopWatch.ElapsedMilliseconds.ToString();
                SeleniumHelper.Cleanup(testInfo.Browser, testInfo.Language);
                stopWatch.Stop();
                _log.Info($"------- {testResult.Status} -------");
            }
            return testResult;
        }

        private void LoadTasks(TestInfo testInfo)
        {
            // Remove dirty tests to reload test tasks
            testInfo.Tasks.Clear();
            var jsonTasks = GetTaskJsonData(testInfo.Name, Path.Combine("Testcases", testInfo.Plugin ?? ""));
            if (string.IsNullOrEmpty(jsonTasks)) Assert.Fail("No test file to execute");
            var tasks = JsonConvert.DeserializeObject<List<TaskActionInfo>>(jsonTasks, new TestJsonConverter());
            AddTasks(tasks, testInfo, "Task");
            _log.Info($"{testInfo.Tasks.Count} tasks loaded.");
        }

        private void AddTasks(IEnumerable<TaskActionInfo> tasks, TestInfo testInfo, string taskNamePrefix)
        {
            var counter = 0;
            foreach (var actionInfo in tasks)
            {
                if (actionInfo.Task.Environments != null && !actionInfo.Task.Environments.Contains(Constants.AppConfig.BuildEnvironment))
                {
                    _log.Info($"Task {actionInfo.Action} doesn't have a env '{Constants.AppConfig.BuildEnvironment}' so its excluded.");
                    continue;
                }
                counter++;
                if (actionInfo.Action == ActionType.AddTemplate)
                {
                    AddTemplateTasks(actionInfo.Task, testInfo, _execContext.Storage, $"{taskNamePrefix} {counter}::");
                }
                else
                {
                    actionInfo.Task.TaskName = $"{taskNamePrefix} {counter}";
                    actionInfo.Task.Parent = testInfo;
                    testInfo.Tasks.Add(actionInfo.Task);

                }
            }
        }

        private void AddTemplateTasks(TaskInfo task, TestInfo testInfo, TestStorage storage, string taskNamePrefix)
        {
            if (!(task is TemplateTaskInfo)) return;

            var templateTask = (TemplateTaskInfo)task;
            if (templateTask.Parameters != null)
            {
                foreach (var parameter in templateTask.Parameters)
                {
                    var finalValue = Utilities.ConfigureUrl(parameter.Value);
                    storage.Cache.AddOrUpdate(parameter.Key, finalValue);
                }
            }
            var jsonTasks = GetTaskJsonData(templateTask.Name, "Templates");
            if (string.IsNullOrEmpty(jsonTasks)) Assert.Fail("No template file to include");
            var tasks = JsonConvert.DeserializeObject<List<TaskActionInfo>>(jsonTasks, new TestJsonConverter());
            AddTasks(tasks, testInfo, $"{taskNamePrefix}{templateTask.Name}::SubTask");
        }

        private string GetTaskJsonData(string fileName, string folderPath)
        {
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            var testName = fileName + ".json";
            var jsonFileDir = Path.Combine(Constants.AppConfig.TestDataBasePath, folderPath);
            if (!Directory.Exists(jsonFileDir))
            {
                _log.Error($"Folder not found, Path={jsonFileDir}");
                return string.Empty;
            }
            _log.Info($"Template loaded: {fileName}");
            var jsonFile = Directory.GetFiles(jsonFileDir, testName, SearchOption.AllDirectories);
            if (jsonFile.Length > 0) return File.ReadAllText(jsonFile[0]);
            _log.Error($"File not found, Path={jsonFileDir}\\{testName}");
            return string.Empty;
        }

        private void PersistLog(TestInfo testInfo)
        {
            var log = _execContext.Storage.Cache.Contains(Constants.Log) ? _execContext.Storage.Cache.Get(Constants.Log) : string.Empty;
            if (!string.IsNullOrEmpty(log))
            {
                var consoleDir = LogHelper.LogsBasePath + "\\Console";
                if (!Directory.Exists(consoleDir)) Directory.CreateDirectory(consoleDir);
                var fileName = Path.Combine(consoleDir, $"{testInfo.Name}.log");
                File.WriteAllText(fileName, log);
                _log.Info($"ConsoleLog: {new Uri(Path.GetFullPath(fileName)).AbsoluteUri}");
            }
            else
            {
                _log.Info("ConsoleLog: No logs available yet.");
            }
        }
    }
}
