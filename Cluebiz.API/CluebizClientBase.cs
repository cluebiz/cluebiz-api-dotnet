﻿using Cluebiz.API.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
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

        protected string serverUrl;

        internal CluebizClientBase(string serverAddress, string userId, string key)
        {
            this.userId = userId;
            this.key = key;
            this.serverUrl = serverAddress;
            HttpClientHandler handler = new HttpClientHandler();
            handler.Proxy = WebRequest.GetSystemWebProxy();
            handler.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            client = new HttpClient(handler);

            client.BaseAddress = new Uri(serverAddress + (serverAddress.EndsWith("/") ? "" : "/") + "REST/");
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

        protected async Task PostChunks( string base64,Guid clueBizfileId, Guid? cluebizClientId = null)
        {
        
            if (cluebizClientId.HasValue) cluebizClientId.ToString();
            var cluebizToken = await GetToken();

            var postBody = new
            {
                cmd = "fileuploadchunksend",
                token = cluebizToken,
                clientId = cluebizClientId,
                fileId = clueBizfileId,
                data = base64
            };

            string jsonPostBody = JsonConvert.SerializeObject(postBody);
            var content = new StringContent(jsonPostBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("?",content);
            string responseContent = await response.Content.ReadAsStringAsync();
        }


        ~CluebizClientBase()
        {
            client?.Dispose();
        }
    }
}
