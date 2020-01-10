using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bromine.Automation.Core.Enum;
using Bromine.Automation.Core.Helpers;
using Bromine.RestServiceWrapper.Models;
using Newtonsoft.Json;
using Response = Bromine.RestServiceWrapper.Models.Response;

namespace Bromine.RestServiceWrapper.Helpers
{
    public static class RestHelper
    {
        private static readonly HttpHelper HttpHelper;

        static RestHelper()
        {
            HttpHelper = new HttpHelper();
        }

        public static async Task<HttpResponseMessage> GetApiResponse(string url, Dictionary<string, string> headers, string payload)
        {
            var logger = LogHelper.GetLogger();
            var requestLog = $"Request: Url={url}, " +
                             $"Header(s)={string.Join("", headers.Select(kvp => kvp.Key + ": " + kvp.Value.ToString()))}, " +
                             $"Payload={payload}";
            logger.Debug(requestLog);
            HttpResponseMessage resp = null;
            try
            {
                resp = await HttpHelper.PostAsync(url, payload, headers);
            }
            catch (HttpRequestException ex)
            {
                logger.Error($"Code={(int)resp.StatusCode}, Details={ await resp.Content.ReadAsStringAsync()}, Request={requestLog}", ex);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return resp;
        }

        public static bool ValidateResponse(string responseBody, Response expectedResult)
        {
            switch (expectedResult.Type)
            {
                case RestResponseType.CreateOidcTokenResponse:
                    var actualOidcResp = JsonConvert.DeserializeObject<CreateOidcTokenResponse>(responseBody);
                    var expectedOidcResp = JsonConvert.DeserializeObject<CreateOidcTokenResponse>(expectedResult.Value.ToString());
                    return new CreateOidcTokenResponseComparer().Equals(expectedOidcResp, actualOidcResp);
                default:
                    throw new NotImplementedException("Unknown Rest Response");
            }
        }
    }
}
