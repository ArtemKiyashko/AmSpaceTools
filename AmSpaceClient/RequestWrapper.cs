using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AmSpaceModels;
using Newtonsoft.Json;

namespace AmSpaceClient
{

    public class RequestWrapper : IRequestWrapper
    {
        public CookieContainer CookieContainer { get; private set; }
        public HttpClient AmSpaceHttpClient { get; private set; }

        public RequestWrapper()
        {
            CookieContainer = new CookieContainer();

            var handler = new HttpClientHandler()
            {
                CookieContainer = CookieContainer
            };

            AmSpaceHttpClient = new HttpClient(handler);
        }

        public void AddAuthHeaders(AuthenticationHeaderValue authData)
        {
            AmSpaceHttpClient.DefaultRequestHeaders.Authorization = authData;
        }

        public void AddAuthCookies(Uri uri, Cookie cookie)
        {
            CookieContainer.Add(uri, cookie);
        }

        private StringContent PrepareContent<TInput>(TInput model)
        {
            var stringContent = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var httpcontent = new StringContent(stringContent);
            httpcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpcontent;
        }

        public async Task<T> GetAsyncWrapper<T>(string endpoint) where T : class
        {
            var result = await AmSpaceHttpClient.GetAsync(endpoint);
            return await result.ValidateAsync<T>();
        }

        public async Task<HttpResponseMessage> GetAsyncWrapper(string endpoint)
        {
            var result = await AmSpaceHttpClient.GetAsync(endpoint);
            return result;
        }

        public async Task<bool> PutAsyncWrapper<T>(T model, string endpoint)
        {
            var httpcontent = PrepareContent(model);
            var result = await AmSpaceHttpClient.PutAsync(endpoint, httpcontent);
            return await result.ValidateAsync();
        }

        public async Task<bool> DeleteAsyncWrapper<T>(T model, string endpoint)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(endpoint),
                Content = PrepareContent(model)
            };
            var result = await AmSpaceHttpClient.SendAsync(request);
            return await result.ValidateAsync();
        }

        public async Task<bool> DeleteAsyncWrapper(string endpoint)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(endpoint),
            };
            var result = await AmSpaceHttpClient.SendAsync(request);
            return await result.ValidateAsync();
        }

        public async Task<TOutput> PostAsyncWrapper<TInput, TOutput>(TInput model, string endpoint) where TOutput : class
        {
            var httpcontent = PrepareContent(model);
            var result = await AmSpaceHttpClient.PostAsync(endpoint, httpcontent);
            return await result.ValidateAsync<TOutput>();
        }

        public async Task<HttpResponseMessage> PostAsyncWrapper(string endpoint, FormUrlEncodedContent content)
        {
            return await AmSpaceHttpClient.PostAsync(endpoint, content);
        }

        public async Task<TOutput> PostAsyncWrapper<TOutput>(string endpoint, FormUrlEncodedContent content) where TOutput : class
        {
            var result = await AmSpaceHttpClient.PostAsync(endpoint, content);
            return await result.ValidateAsync<TOutput>();
        }

        public async Task<TOutput> PatchAsyncWrapper<TInput, TOutput>(TInput model, string endpoint) where TOutput : class
        {
            var request = CreateHttpMessage(model, endpoint, "PATCH");
            var result = await AmSpaceHttpClient.SendAsync(request);
            return await result.ValidateAsync<TOutput>();
        }

        private HttpRequestMessage CreateHttpMessage<TInput>(TInput model, string endpoint, string method)
        {
            return new HttpRequestMessage
            {
                Method = new HttpMethod(method),
                RequestUri = new Uri(endpoint),
                Content = PrepareContent(model)
            };
        }

        public async Task<TOutput> PutAsyncWrapper<TInput, TOutput>(TInput model, string endpoint) where TOutput : class
        {
            var httpcontent = PrepareContent(model);
            var result = await AmSpaceHttpClient.PutAsync(endpoint, httpcontent);
            return await result.ValidateAsync<TOutput>();
        }

        public async Task<bool> PatchAsyncWrapper<TInput>(TInput model, string endpoint)
        {
            var request = CreateHttpMessage(model, endpoint, "PATCH");
            var result = await AmSpaceHttpClient.SendAsync(request);
            return await result.ValidateAsync();
        }
    }
}
