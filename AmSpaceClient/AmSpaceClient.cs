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
        
        public bool IsAthorized { get; private set; }
        public LoginResult LoginResult { get; private set; }
        public string ClientId { get; private set; }
        public string GrantPermissionType { get; private set; }
        public ApiEndpoints Endpoints { get; private set; }
        private IRequestsWrapper _requestsWrapper { get; set; }
        
        
        public AmSpaceClient(IRequestsWrapper wrapper)
        {
            _requestsWrapper = wrapper;
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
            var result = await _requestsWrapper.PostAsyncWrapper(Endpoints.TokenEndpoint, content);
            LoginResult = await result.ValidateAsync<LoginResult>();
            _requestsWrapper.AddAuthHeaders(new AuthenticationHeaderValue("Bearer", LoginResult.AccessToken));
            _requestsWrapper.AddAuthCookies(new Uri(Endpoints.BaseAddress), new Cookie("accessToken", LoginResult.AccessToken));
            IsAthorized = true;
            return true;
        }

        public async Task<BitmapSource> GetAvatarAsync(string link)
        {
            if (!IsAthorized) throw new UnauthorizedAccessException();
            var result = await _requestsWrapper.GetAsyncWrapper(link);
            if (!result.IsSuccessStatusCode)
                result = await _requestsWrapper.GetAsyncWrapper($"{Endpoints.BaseAddress}/static/avatar.png");
            var content = await result.Content.ReadAsByteArrayAsync();
            return (BitmapSource)new ImageSourceConverter().ConvertFrom(content);
        }

        public async Task<IEnumerable<Competency>> GetCompetenciesAsync()
        {
            var pager = await _requestsWrapper.GetAsyncWrapper<CompetencyPager>(Endpoints.CompetencyEndpoint);
            var allComps = new List<Competency>();
            allComps.AddRange(pager.Results);
            while (!string.IsNullOrEmpty(pager.Next))
            {
                pager = await _requestsWrapper.GetAsyncWrapper<CompetencyPager>(pager.Next);
                allComps.AddRange(pager.Results);
            }
            return allComps;
        }

        public async Task<CompetencyAction> GetCompetencyActionsAsync(long competencyId)
        {
            return await _requestsWrapper.GetAsyncWrapper<CompetencyAction>(string.Format(Endpoints.CompetecyActionEndpoint, competencyId.ToString()));
        }

        public async Task<IEnumerable<Level>> GetLevelsAsync()
        {
            return await _requestsWrapper.GetAsyncWrapper<IEnumerable<Level>>(Endpoints.LevelsEndpoint);
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
            var result = await _requestsWrapper.PostAsyncWrapper(Endpoints.LogoutEndpoint, content);
            await result.ValidateAsync();
            IsAthorized = false;
            return true;
        }

        public async Task<Profile> ProfileRequestAsync(int? profileId = null)
        {
            var endpoint = Endpoints.ProfileEndpoint + (profileId.HasValue ? profileId.Value.ToString() : "");
            var result = await _requestsWrapper.GetAsyncWrapper(endpoint);
            return await result.ValidateAsync<Profile>();
        }

        public async Task<bool> UpdateActionAsync(UpdateAction model, long competencyId)
        {
            var endpoint = string.Format(Endpoints.UpdateActionEndpoint, competencyId.ToString());
            return await _requestsWrapper.PutAsyncWrapper(model, endpoint);
        }

        public async Task<IEnumerable<AmspaceDomain>> GetOrganizationStructureAsync()
        {
            return await _requestsWrapper.GetAsyncWrapper<IEnumerable<AmspaceDomain>>(Endpoints.DomainNodesEndpoint);
        }

        public async Task<IEnumerable<AmspaceUser>> GetDomainUsersAsync(int domainId)
        {
            return await _requestsWrapper.GetAsyncWrapper<IEnumerable<AmspaceUser>>(Endpoints.UsersInDomainEndpoint + domainId);
        }

        public async Task<bool> PutUserAsync(SapUser user)
        {
            return await _requestsWrapper.PutAsyncWrapper(user, Endpoints.UserSapEndpoint);
        }

        public async Task<bool> PutDomainAsync(SapDomain domain)
        {
            return await _requestsWrapper.PutAsyncWrapper(domain, Endpoints.DomainSapEndpoint);
        }

        public async Task<bool> DisableUserAsync(SapUserDelete user)
        {
            return await _requestsWrapper.DeleteAsyncWrapper(user, Endpoints.UserSapEndpoint);
        }
    }
}
