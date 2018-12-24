using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AmSpaceModels;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace AmSpaceClient
{

    public class RequestWrapper : IRequestWrapper
    {
        public CookieContainer CookieContainer { get; private set; }
        public virtual HttpClient AmSpaceHttpClient { get; private set; }
        private IAsyncPolicy<HttpResponseMessage> _httpResponcePolicy;
        public IAsyncPolicy<HttpResponseMessage> HttpResponcePolicy
        {
            get => _httpResponcePolicy;
            set
            {
                if (value != null)
                    _httpResponcePolicy = value;
            }
        }

        public RequestWrapper()
        {
            CookieContainer = new CookieContainer();

            var handler = new HttpClientHandler()
            {
                CookieContainer = CookieContainer
            };

            AmSpaceHttpClient = new HttpClient(handler);
            HttpResponcePolicy = GetDefaultPolicy();
        }
        public void AddAuthHeaders(AuthenticationHeaderValue authData)
        {
            AmSpaceHttpClient.DefaultRequestHeaders.Authorization = authData;
        }

        public void AddAuthCookies(Uri uri, Cookie cookie)
        {
            CookieContainer.Add(uri, cookie);
        }

        public async Task<T> GetAsyncWrapper<T>(string endpoint) where T : class
        {
            var result = await HttpResponcePolicy.ExecuteAsync(async () => await AmSpaceHttpClient.GetAsync(endpoint));
            return await result.ValidateAsync<T>();
        }

        public async Task<HttpResponseMessage> GetAsyncWrapper(string endpoint)
        {
            return await HttpResponcePolicy.ExecuteAsync(async () => await AmSpaceHttpClient.GetAsync(endpoint));
        }
        
        public async Task<bool> PutAsyncWrapper<T>(T model, string endpoint)
        {
            var result = await HttpResponcePolicy.ExecuteAsync(async () =>
            {
                var httpcontent = PrepareStringContent(model);
                return await AmSpaceHttpClient.PutAsync(endpoint, httpcontent);
            });
            return await result.ValidateAsync();
        }
        public async Task<TOutput> PutAsyncWrapper<TInput, TOutput>(TInput model, string endpoint) where TOutput : class
        {
            var result = await HttpResponcePolicy.ExecuteAsync(async () =>
            {
                var httpcontent = PrepareStringContent(model);
                return await AmSpaceHttpClient.PutAsync(endpoint, httpcontent);
            });
            return await result.ValidateAsync<TOutput>();
        }

        public async Task<bool> DeleteAsyncWrapper<T>(T model, string endpoint)
        {
            var result = await HttpResponcePolicy.ExecuteAsync(async () =>
            {
                var request = CreateHttpMessage<T>(model, endpoint, "DELETE"); 
                return await AmSpaceHttpClient.SendAsync(request);
            });
            return await result.ValidateAsync();
        }

        public async Task<bool> DeleteAsyncWrapper(string endpoint)
        {
            var result = await HttpResponcePolicy.ExecuteAsync(async () =>
            {
                var request = CreateHttpMessage<BaseModel>(null, endpoint, "DELETE");
                return await AmSpaceHttpClient.SendAsync(request);
            });
            return await result.ValidateAsync();
        }

        public async Task<TOutput> PostAsyncWrapper<TInput, TOutput>(TInput model, string endpoint) where TOutput : class
        {
            var result = await HttpResponcePolicy.ExecuteAsync(async () =>
            {
                var httpcontent = PrepareStringContent(model);
                return await AmSpaceHttpClient.PostAsync(endpoint, httpcontent);
            });
            return await result.ValidateAsync<TOutput>();
        }

        public async Task<bool> PostAsyncWrapper<TInput>(TInput model, string endpoint)
        {
            var result = await HttpResponcePolicy.ExecuteAsync(async () =>
            {
                var httpcontent = PrepareStringContent(model);
                return await AmSpaceHttpClient.PostAsync(endpoint, httpcontent);
            });
            return await result.ValidateAsync();
        }

        public async Task<HttpResponseMessage> PostFormUrlEncodedContentAsyncWrapper(IEnumerable<KeyValuePair<string, string>> content, string endpoint)
        {
            return await HttpResponcePolicy.ExecuteAsync(async () =>
            {
                var contentCopy = new FormUrlEncodedContent(content);
                return await AmSpaceHttpClient.PostAsync(endpoint, contentCopy);
            });
        }

        public async Task<TOutput> PostFormUrlEncodedContentAsyncWrapper<TOutput>(IEnumerable<KeyValuePair<string, string>> content, string endpoint) where TOutput : class
        {
            var result = await HttpResponcePolicy.ExecuteAsync(async () =>
            {
                var contentCopy = new FormUrlEncodedContent(content);
                return await AmSpaceHttpClient.PostAsync(endpoint, contentCopy);
            });
            return await result.ValidateAsync<TOutput>();
        }

        public async Task<TOutput> PatchAsyncWrapper<TInput, TOutput>(TInput model, string endpoint) where TOutput : class
        {
            var result = await HttpResponcePolicy.ExecuteAsync(async () =>
            {
                var request = CreateHttpMessage(model, endpoint, "PATCH");
                return await AmSpaceHttpClient.SendAsync(request);
            });
            return await result.ValidateAsync<TOutput>();
        }

        public async Task<bool> PatchAsyncWrapper<TInput>(TInput model, string endpoint)
        {
            var result = await HttpResponcePolicy.ExecuteAsync(async () =>
            {
                var request = CreateHttpMessage(model, endpoint, "PATCH");
                return await AmSpaceHttpClient.SendAsync(request);
            });
            return await result.ValidateAsync();
        }
        private HttpRequestMessage CreateHttpMessage<TInput>(TInput model, string endpoint, string method)
        {
            return new HttpRequestMessage
            {
                Method = new HttpMethod(method),
                RequestUri = new Uri(endpoint),
                Content = PrepareStringContent(model)
            };
        }

        private StringContent PrepareStringContent<TInput>(TInput model)
        {
            if (model == null)
                return null;
            var stringContent = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "yyyy-MM-dd"
            });
            var httpcontent = new StringContent(stringContent);
            httpcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpcontent;
        }
        private Policy<HttpResponseMessage> GetDefaultPolicy()
        {
            var statusCodeToHandle = new[] { HttpStatusCode.ServiceUnavailable, HttpStatusCode.BadGateway };
            return Policy.HandleResult<HttpResponseMessage>(responce => statusCodeToHandle.Contains(responce.StatusCode))
                .WaitAndRetryAsync(3, (attempt) => TimeSpan.FromMilliseconds(attempt * attempt * 1000));
        }
    }
}
