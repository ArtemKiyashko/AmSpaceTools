using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceClient
{
    public interface IRequestsWrapper
    {
        CookieContainer CookieContainer { get; }
        HttpClient AmSpaceHttpClient { get; }
        void AddAuthHeaders(AuthenticationHeaderValue authData);
        void AddAuthCookies(Uri uri, Cookie cookie);
        // returns validated object of type T
        Task<T> GetAsyncWrapper<T>(string endpoint) where T : class;
        // returns unvalidated raw HttpResponse
        Task<HttpResponseMessage> GetAsyncWrapper(string endpoint);
        Task<bool> PutAsyncWrapper<T>(T model, string endpoint);
        Task<bool> DeleteAsyncWrapper<T>(T model, string endpoint);
        Task<bool> DeleteAsyncWrapper(string endpoint);
        Task<TOutput> PostAsyncWrapper<TInput, TOutput>(TInput model, string endpoint) where TOutput : class;
        Task<TOutput> PostAsyncWrapper<TOutput>(string endpoint, FormUrlEncodedContent content) where TOutput : class;
        Task<HttpResponseMessage> PostAsyncWrapper(string endpoint, FormUrlEncodedContent content);
        Task<TOutput> PatchAsyncWrapper<TInput, TOutput>(TInput model, string endpoint) where TOutput : class;
    }
}
