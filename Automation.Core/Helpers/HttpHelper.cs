using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Bromine.Automation.Core.Common;
using log4net;
using Newtonsoft.Json;

namespace Bromine.Automation.Core.Helpers
{
    public class HttpHelper
    {
        private readonly ILog _logger = LogHelper.GetLogger();

        static HttpHelper()
        {
            Client ??= new HttpClient();
        }

        private static readonly HttpClient Client;

        #region Public Members

        public async Task<T> GetAsync<T>(string uri, IDictionary<string, string> headers)
        {
            return await InvokeAsync(client => client.GetAsync(uri), headers, ParseResponse<T>);
        }

        public async Task<HttpResponseMessage> GetAsync(string uri, IDictionary<string, string> headers)
        {
            return await InvokeAsync<HttpResponseMessage>(client => client.GetAsync(uri), headers);
        }

        public async Task<T> PostAsync<T>(string uri, string data, IDictionary<string, string> headers)
        {
            return await InvokeAsync(client => PostAsync(client, uri, data, headers), headers, ParseResponse<T>);
        }

        public async Task<HttpResponseMessage> PostAsync(string uri, string data, IDictionary<string, string> headers)
        {
            return await InvokeAsync<HttpResponseMessage>(client => PostAsync(client, uri, data, headers), headers);
        }

        public async Task<T> DeleteAsync<T>(string uri, IDictionary<string, string> headers)
        {
            return await InvokeAsync(client => client.DeleteAsync(uri), headers, ParseResponse<T>);
        }

        public Dictionary<string, string> GetQueryStringDictionary(string url)
        {
            var queryStringValueDic = new Dictionary<string, string>();
            try
            {
                //the HttpUtility.ParseQueryString will decode the value automatically, if to keep encode, use the Utilities.ParseQueryString
                var collection = Utilities.ParseQueryString(Utilities.RemoveReservedCharacters(url));
                foreach (var itemKey in collection.AllKeys)
                {
                    if (string.IsNullOrEmpty(itemKey)) continue;
                    var itemValue = collection[itemKey];
                    queryStringValueDic.Add(itemKey, itemValue);
                    _logger.Debug($"value extracted from query string key {itemKey}: {itemValue}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception on get all query strings from url {url}. Exception message: {ex.Message}");
            }

            return queryStringValueDic;
        }

        #endregion

        #region Private Members

        private static async Task<HttpResponseMessage> PostAsync(HttpClient client, string uri, string data, IDictionary<string, string> headers)
        {
            using var content = new StringContent(data, Encoding.UTF8);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse(headers[Constants.HeaderContentType]);
            return await client.PostAsync(uri, content);
        }

        private static async Task<T> ParseResponse<T>(HttpResponseMessage response)
        {
            var resp = await response.Content.ReadAsStringAsync();
            if (typeof(T) == typeof(string))
            {
                return (T)(object)resp;
            }

            return JsonConvert.DeserializeObject<T>(resp);
        }

        private async Task<T> InvokeAsync<T>(Func<HttpClient, Task<HttpResponseMessage>> operation,
          IDictionary<string, string> headers, Func<HttpResponseMessage, Task<T>> actionOnResponse)
        {
            var response = await InvokeAsync<T>(operation, headers);
            if (!response.IsSuccessStatusCode)
            {
                var exception = new HttpRequestException($"Server returned an error. StatusCode : {response.StatusCode}");
                exception.Data.Add("StatusCode", response.StatusCode);
                exception.Data.Add("Content", await response.Content.ReadAsStringAsync());
                throw exception;
            }

            if (actionOnResponse != null)
            {
                return await actionOnResponse(response);
            }

            return default;
        }

        private async Task<HttpResponseMessage> InvokeAsync<T>(Func<HttpClient, Task<HttpResponseMessage>> operation,
          IDictionary<string, string> headers)
        {
            if (operation == null) throw new ArgumentNullException(nameof(operation));

            if (headers != null)
            {
                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Accept.Clear();
                foreach (var (key, value) in headers)
                {
                    switch (key.ToLower())
                    {
                        case Constants.HeaderContentType:
                            // Ignoring it since its added to the request content itself
                            break;
                        case Constants.HeaderAccept:
                            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(value));
                            break;
                        case Constants.HeaderHost:
                            Client.DefaultRequestHeaders.Host = value;
                            break;
                        case Constants.HeaderAuthorization:
                            if (AuthenticationHeaderValue.TryParse(value, out var authHeaderValue))
                            {
                                Client.DefaultRequestHeaders.Authorization = authHeaderValue;
                            }
                            else
                            {
                                Client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
                            }
                            break;
                        default:
                            Client.DefaultRequestHeaders.Add(key, value);
                            break;
                    }
                }
            }

            var response = await operation(Client);
            return response;
        }

        #endregion

    }

}