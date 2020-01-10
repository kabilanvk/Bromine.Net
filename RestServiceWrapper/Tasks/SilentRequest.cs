using System;
using System.Linq;
using System.Threading.Tasks;
using Bromine.Automation.Core.Common;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Models;
using Bromine.RestServiceWrapper.Helpers;
using Bromine.RestServiceWrapper.Models;
using Newtonsoft.Json;
using Response = Bromine.RestServiceWrapper.Models.Response;

namespace Bromine.RestServiceWrapper.Tasks
{
    public class SilentRequest : BaseTask
    {
        public override async Task<TaskResult> Process(TaskInfo task)
        {
            if (!(task is SilentTaskInfo))
                throw new ArgumentException($"Expected SilentTaskInfo but passed {task.GetType().Name}");

            var result = new TaskResult(false);
            var taskInfo = (SilentTaskInfo)task;
            try
            {
                var responseKey = taskInfo.CacheKey;
                // Get the test data by test case
                var url = Utilities.ConfigureUrl(taskInfo.Request.Url);
                if (string.IsNullOrEmpty(url)) throw new Exception();
                // Replace the custom header list with values from config if needed. construct new header list
                var headers = taskInfo.Request.Header.FindAll(h => h.Value.Equals("?", StringComparison.InvariantCultureIgnoreCase));
                // Populate header parameters from cache 
                var headerList = taskInfo.Request.Header.ToDictionary(header => header.Key.ToLower(), header => Storage.Cache.ReplaceCacheKeys(header.Value));
                var payload = headerList[Constants.HeaderContentType].Equals(Constants.ContentTypeJson)
                  ? JsonConvert.SerializeObject(taskInfo.Request.Payload)
                  : taskInfo.Request.Payload.ToString();
                // Populate payload parameters from cache 
                var finalPayload = Storage.Cache.ReplaceCacheKeys(payload);
                // Populate response parameters from cache 
                var expected = JsonConvert.DeserializeObject<Response>(Storage.Cache.ReplaceCacheKeys(JsonConvert.SerializeObject(taskInfo.Response)));
                // Verify response
                var response = await RestHelper.GetApiResponse(url, headerList, finalPayload);
                
                if (response == null) return result;
                var responseStr = await response.Content.ReadAsStringAsync();

                if (responseKey.IsNotEmpty())
                {
                    Storage.Cache.Add(responseKey, responseStr);
                    Logger.Info($"Rest template response is cached at '{responseKey}'");
                }
                Logger.Debug($"Rest call response: {responseStr}");
                if(expected.StatusCode.HasValue)
                {
                    result.Success = expected.StatusCode.Value == (int)response.StatusCode;
                }
                if (responseStr.IsEmpty()) return result;
                result.Success = RestHelper.ValidateResponse(responseStr, expected);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return result;
            }
        }
    }
}
