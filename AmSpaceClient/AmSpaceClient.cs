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
            if (result.StatusCode != HttpStatusCode.OK) return false;
            var resultContent = await result.Content.ReadAsStringAsync();
            LoginResult = JsonConvert.DeserializeObject<LoginResult>(resultContent);
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
            if (!IsAthorized) throw new UnauthorizedAccessException();
            var result = await AmSpaceHttpClient.GetAsync(Endpoints.CompetencyEndpoint);
            if (!result.IsSuccessStatusCode) throw new Exception("something go wrong while getting Competencies");
            var content = await result.Content.ReadAsStringAsync();
            var pager = JsonConvert.DeserializeObject<CompetencyPager>(content);
            var allComps = new List<Competency>();
            allComps.AddRange(pager.Results);
            while (!string.IsNullOrEmpty(pager.Next))
            {
                result = await AmSpaceHttpClient.GetAsync(pager.Next);
                content = await result.Content.ReadAsStringAsync();
                pager = JsonConvert.DeserializeObject<CompetencyPager>(content);
                allComps.AddRange(pager.Results);
            }
            return allComps;
        }

        public async Task<CompetencyAction> GetCompetencyActionsAsync(long competencyId)
        {
            if (!IsAthorized) throw new UnauthorizedAccessException();
            var endpoint = string.Format(Endpoints.CompetecyActionEndpoint, competencyId.ToString());
            var result = await AmSpaceHttpClient.GetAsync(endpoint);
            if (!result.IsSuccessStatusCode) throw new Exception("something go wrong while getting Compenetcy Actions");
            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CompetencyAction>(content);
        }

        public async Task<IEnumerable<Level>> GetLevelsAsync()
        {
            if (!IsAthorized) throw new UnauthorizedAccessException();
            var result = await AmSpaceHttpClient.GetAsync(Endpoints.LevelsEndpoint);
            if (!result.IsSuccessStatusCode) throw new Exception("something go wrong while getting levels");
            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Level>>(content);
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
            if (result.StatusCode != HttpStatusCode.OK) return false;
            IsAthorized = false;
            return true;
        }

        public async Task<Profile> ProfileRequestAsync()
        {
            var result = AmSpaceHttpClient.GetAsync(Endpoints.ProfileEndpoint, HttpCompletionOption.ResponseContentRead);
            var stringResult = await result.Result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Profile>(stringResult);
        }

        public async Task<bool> UpdateActionAsync(UpdateAction model, long competencyId)
        {
            if (!IsAthorized) throw new UnauthorizedAccessException();
            var endpoint = string.Format(Endpoints.UpdateActionEndpoint, competencyId.ToString());
            var stringContent = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var httpcontent = new StringContent(stringContent);
            httpcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await AmSpaceHttpClient.PutAsync(endpoint, httpcontent);
            if (result.StatusCode != HttpStatusCode.OK) throw new Exception("something go wrong while updating Actions");
            return true;
        }

        private void AddAuthHeaders()
        {
            AmSpaceHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginResult.AccessToken);
        }

        private void AddAuthCookies()
        {
            CookieContainer.Add(new Uri(Endpoints.BaseAddress), new Cookie("accessToken", LoginResult.AccessToken));
        }

        public Task<IEnumerable<SapDomain>> GetOrganizationStructureAsync(int rootMpk)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SapUser>> GetUnitUsersAsync(int unitMpk)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PutUserAsync(SapUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PutDomainAsync(SapDomain domain)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DisableUserAsync(SapUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DisableDomainAsync(SapDomain domain)
        {
            throw new NotImplementedException();
        }
    }
}
