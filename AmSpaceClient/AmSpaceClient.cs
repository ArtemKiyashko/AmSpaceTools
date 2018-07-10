using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AmSpaceModels;
using AmSpaceTools.Infrastructure;
using Newtonsoft.Json;

namespace AmSpaceClient
{
    public class AmSpaceClient : IAmSpaceClient
    {
        public CookieContainer CookieContainer { get; private set; }
        public bool IsAthorized { get; private set; }
        public LoginResult LoginResult { get; private set; }
        public string ClientId { get; private set; }
        public string GrantPermissionType { get; private set; }
        public ApiEndpoints Endpoints { get; private set; }
        public HttpClient AmSpaceHttpClient { get; private set; }
        
        public AmSpaceClient()
        {
            CookieContainer = new CookieContainer();

            var handler = new HttpClientHandler()
            {
                CookieContainer = CookieContainer
            }; 

            AmSpaceHttpClient = new HttpClient(handler);
            IsAthorized = false;
        }

        private void AddAuthHeaders()
        {
            AmSpaceHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginResult.AccessToken);
        }

        private void AddAuthCookies()
        {
            CookieContainer.Add(new Uri(Endpoints.BaseAddress), new Cookie("accessToken", LoginResult.AccessToken));
        }

        public async Task<bool> LoginRequestAsync(string userName, SecureString password, IAmSpaceEnvironment environment)
        {
            if (IsAthorized) return true;
            Endpoints = new ApiEndpoints(environment.BaseAddress);
            ClientId = environment.ClientId;
            GrantPermissionType = environment.GrantPermissionType;
            var values = new Dictionary<string, string>
                {
                    { "username", userName },
                    { "password", password.ToInsecureString() },
                    { "grant_type", GrantPermissionType },
                    { "client_id", ClientId }
                };
            var content = new FormUrlEncodedContent(values);
            var result = await AmSpaceHttpClient.PostAsync(Endpoints.TokenEndpoint, content);
            LoginResult = await result.ValidateAsync<LoginResult>();
            AddAuthHeaders();
            AddAuthCookies();
            IsAthorized = true;
            return true;
        }

        public async Task<BitmapSource> GetAvatarAsync(string link)
        {
            if (!IsAthorized) throw new UnauthorizedAccessException();
            var result = await AmSpaceHttpClient.GetAsync(link);
            if (!result.IsSuccessStatusCode)
                result = await AmSpaceHttpClient.GetAsync($"{Endpoints.BaseAddress}/static/avatar.png");
            var content = await result.Content.ReadAsByteArrayAsync();
            return (BitmapSource)new ImageSourceConverter().ConvertFrom(content);
        }

        public async Task<IEnumerable<Competency>> GetCompetenciesAsync()
        {
            var pager = await GetAsyncWrapper<CompetencyPager>(Endpoints.CompetencyEndpoint);
            var allComps = new List<Competency>();
            allComps.AddRange(pager.Results);
            while (!string.IsNullOrEmpty(pager.Next))
            {
                pager = await GetAsyncWrapper<CompetencyPager>(pager.Next);
                allComps.AddRange(pager.Results);
            }
            var levels = await GetLevelsAsync();
            allComps.ForEach(comp => comp.Level = levels.FirstOrDefault(_ => _.Id == comp.LevelId));
            return allComps;
        }

        public async Task<CompetencyAction> GetCompetencyActionsAsync(long competencyId)
        {
            return await GetAsyncWrapper<CompetencyAction>(string.Format(Endpoints.CompetecyActionEndpoint, competencyId.ToString()));
        }

        public async Task<IEnumerable<Level>> GetLevelsAsync()
        {
            return await GetAsyncWrapper<IEnumerable<Level>>(Endpoints.LevelsEndpoint);
        }

        public async Task<bool> LogoutRequestAsync()
        {
            if (!IsAthorized) return false;
            var values = new Dictionary<string, string>()
                {
                    { "token", LoginResult.AccessToken },
                    { "client_id", ClientId }
                };
            var content = new FormUrlEncodedContent(values);
            var result = await AmSpaceHttpClient.PostAsync(Endpoints.LogoutEndpoint, content);
            await result.ValidateAsync();
            IsAthorized = false;
            return true;
        }

        public async Task<Profile> ProfileRequestAsync(int? profileId = null)
        {
            var endpoint = Endpoints.ProfileEndpoint + (profileId.HasValue ? profileId.Value.ToString() : "");
            var result = await AmSpaceHttpClient.GetAsync(endpoint, HttpCompletionOption.ResponseContentRead);
            return await result.ValidateAsync<Profile>();
        }

        public async Task<bool> UpdateActionAsync(UpdateAction model, long competencyId)
        {
            var endpoint = string.Format(Endpoints.UpdateActionEndpoint, competencyId.ToString());
            return await PutAsyncWrapper(model, endpoint);
        }

        public async Task<IEnumerable<AmspaceDomain>> GetOrganizationStructureAsync()
        {
            return await GetAsyncWrapper<IEnumerable<AmspaceDomain>>(Endpoints.DomainNodesEndpoint);
        }

        public async Task<IEnumerable<AmspaceUser>> GetDomainUsersAsync(int domainId)
        {
            return await GetAsyncWrapper<IEnumerable<AmspaceUser>>(Endpoints.UsersInDomainEndpoint + domainId);
        }

        public async Task<bool> PutUserAsync(SapUser user)
        {
            return await PutAsyncWrapper(user, Endpoints.UserSapEndpoint);
        }

        public async Task<bool> PutDomainAsync(SapDomain domain)
        {
            return await PutAsyncWrapper(domain, Endpoints.DomainSapEndpoint);
        }

        public async Task<bool> DisableUserAsync(SapUserDelete user)
        {
            return await DeleteAsyncWrapper(user, Endpoints.UserSapEndpoint);
        }

        private async Task<T> GetAsyncWrapper<T>(string endpoint) where T : class
        {
            var result = await AmSpaceHttpClient.GetAsync(endpoint);
            return await result.ValidateAsync<T>();
        }

        private async Task<bool> PutAsyncWrapper<T>(T model, string endpoint)
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

        private async Task<bool> DeleteAsyncWrapper<T>(T model, string endpoint)
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

        private async Task<TOutput> PostAsyncWrapper<TInput, TOutput>(TInput model, string endpoint) where TOutput : class
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
    }
}
