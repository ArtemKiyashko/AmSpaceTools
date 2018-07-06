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

    public class RequestsWrapper : IRequestsWrapper
    {
        public CookieContainer CookieContainer { get; private set; }
        public HttpClient AmSpaceHttpClient { get; private set; }

        public RequestsWrapper()
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

        public async Task<T> GetAsyncWrapper<T>(string endpoint) where T : class
        {
            var result = await AmSpaceHttpClient.GetAsync(endpoint);
            return await result.ValidateAsync<T>();
        }

        public async Task<HttpResponseMessage> GetAsyncWrapper(string endpoint)
        {
            return await AmSpaceHttpClient.GetAsync(endpoint);
        }

        public async Task<bool> PutAsyncWrapper<T>(T model, string endpoint)
        {
            var stringContent = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var httpcontent = new StringContent(stringContent);
            httpcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await AmSpaceHttpClient.PutAsync(endpoint, httpcontent);
            return await result.ValidateAsync();
        }

        public async Task<bool> DeleteAsyncWrapper<T>(T model, string endpoint)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(endpoint),
                Content = new StringContent(JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }))
            };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await AmSpaceHttpClient.SendAsync(request);
            return await result.ValidateAsync();
        }

        public async Task<TOutput> PostAsyncWrapper<TInput, TOutput>(TInput model, string endpoint) where TOutput : class
        {
            var stringContent = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var httpcontent = new StringContent(stringContent);
            httpcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await AmSpaceHttpClient.PostAsync(endpoint, httpcontent);
            return await result.ValidateAsync<TOutput>();
        }

        public async Task<HttpResponseMessage> PostAsyncWrapper(string endpoint, FormUrlEncodedContent content)
        {
            return await AmSpaceHttpClient.PostAsync(endpoint, content);
        }
    }
}
