using Cluebiz.API.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Cluebiz.API
{
    internal class CluebizClientBase
    {
        private string userId;
        private string key;

        public HttpClient client;
        private DateTime lastTokenRefresh = DateTime.MinValue;
        private string? token;

        internal CluebizClientBase(string serverAddress, string userId, string key)
        {
            this.userId = userId;
            this.key = key;
            HttpClientHandler handler = new HttpClientHandler();
            handler.Proxy = WebRequest.GetSystemWebProxy();
            handler.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            client = new HttpClient(handler);

            client.BaseAddress = new Uri(CompleteString(serverAddress, "/fragments2/REST"));
        }

        /// <summary>
        /// Stellt sicher das originalString mit der completion endet, und ergänzt alle notwendigen Zeichen
        /// ChatGPT hat das nicht gerallt, ist also gute alte Handarbeit - Sven
        /// </summary>
        private string CompleteString(string originalString, string completion)
        {
            int i;
            for (i = completion.Length; i > 0; i--)
            {
                string part = completion.Substring(0, i);
                if (originalString.EndsWith(part))
                {
                    originalString += completion.Substring(i);
                    break;
                }
            }

            if (i == 0) originalString += completion;
            return originalString;
        }

        private async Task<string> GetToken()
        {
            if (lastTokenRefresh.AddHours(1) < DateTime.UtcNow)
            {
                NameValueCollection queryParams = HttpUtility.ParseQueryString(string.Empty);
                queryParams["cmd"] = "getToken";
                queryParams["userId"] = userId;
                queryParams["restKey"] = key;
                HttpResponseMessage response = await client.GetAsync("?" + queryParams);
                if (response.IsSuccessStatusCode)
                {
                    token = JsonConvert.DeserializeObject<AccessToken>(await response.Content.ReadAsStringAsync())?.Token;
                    lastTokenRefresh = DateTime.UtcNow;
                }
                return token;
            }
            else return token;
        }

        protected async Task<HttpResponseMessage> Get(string cmd, Guid? clientId = null, NameValueCollection queryParams = null)
        {
            if (queryParams == null)
                queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["cmd"] = cmd;
            if (clientId.HasValue)
                queryParams["clientId"] = clientId.ToString();
            queryParams["token"] = await GetToken();

            HttpResponseMessage response = await client.GetAsync("?" + queryParams);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to invoke Cluebiz Api => cmd: {cmd} statusCode: {response.StatusCode}");

            return response;
        }

        protected async Task<T> Get<T>(string cmd, Guid? clientId = null, NameValueCollection queryParams = null)
        {
            HttpResponseMessage response = await Get(cmd, clientId, queryParams);
            string content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
                throw new Exception($"Failed to deserialize Cluebiz Api (result is empty) => cmd: {cmd} statusCode: {response.StatusCode}, content: {content}");

            T? deserialized = JsonConvert.DeserializeObject<T>(content);

            if (deserialized == null)
                throw new Exception($"Failed to deserialize Cluebiz Api (object is null) => cmd: {cmd} statusCode: {response.StatusCode}, content: {content}");

            return deserialized;
        }

        ~CluebizClientBase()
        {
            client?.Dispose();
        }
    }
}
