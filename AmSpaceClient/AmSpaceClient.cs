using AmSpaceModels;
using AmSpaceModels.Idp;
using AmSpaceModels.Organization;
using AmSpaceModels.Performance;
using AmSpaceModels.Sap;
using AmSpaceTools.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UriBuilderExtended;

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

        private async Task<T> GetAsyncWrapper<T>(string endpoint) where T : class
        {
            var result = await AmSpaceHttpClient.GetAsync(endpoint);
            return await result.ValidateAsync<T>();
        }

        private async Task<bool> PutAsyncWrapper<T>(T model, string endpoint)
        {
            var httpcontent = PrepareContent(model);
            var result = await AmSpaceHttpClient.PutAsync(endpoint, httpcontent);
            return await result.ValidateAsync();
        }

        private async Task<bool> DeleteAsyncWrapper<T>(T model, string endpoint)
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

        private async Task<bool> DeleteAsyncWrapper(string endpoint)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(endpoint),
            };
            var result = await AmSpaceHttpClient.SendAsync(request);
            return await result.ValidateAsync();
        }

        private async Task<TOutput> PostAsyncWrapper<TInput, TOutput>(TInput model, string endpoint) where TOutput : class
        {
            var httpcontent = PrepareContent(model);
            var result = await AmSpaceHttpClient.PostAsync(endpoint, httpcontent);
            return await result.ValidateAsync<TOutput>();
        }

        private async Task<TOutput> PatchAsyncWrapper<TInput, TOutput>(TInput model, string endpoint) where TOutput : class
        {
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"),
                RequestUri = new Uri(endpoint),
                Content = PrepareContent(model)
            };
            var result = await AmSpaceHttpClient.SendAsync(request);
            return await result.ValidateAsync<TOutput>();
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
            var pager = await GetAsyncWrapper<CompetencyPager>(Endpoints.CompetencyAdminEndpoint);
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
            return await GetAsyncWrapper<CompetencyAction>(string.Format(Endpoints.CompetecyActionAdminEndpoint, competencyId.ToString()));
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

        public async Task<Profile> GetProfileAsync()
        {
            return await GetAsyncWrapper<Profile>(Endpoints.ProfileEndpoint);
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

        public async Task<IEnumerable<Position>> GetPositionsInLevelsAsync(IEnumerable<Level> levels)
        {
            var url = new UriBuilder(Endpoints.PositionsEndpoint);
            var levelsString = levels.Select(_ => _.Id.ToString()).Aggregate((curr, next) => $"{curr},{next}");
            url.AddQuery("levels", levelsString);
            return await GetAsyncWrapper<IEnumerable<Position>>(url.ToString());
        }

        public async Task<IEnumerable<Position>> GetPositionsAsync()
        {
            return await GetAsyncWrapper<IEnumerable<Position>>(Endpoints.PositionsEndpoint);
        }

        public async Task<IEnumerable<People>> GetPeopleAsync()
        {
            return await GetAsyncWrapper<IEnumerable<People>>(Endpoints.PeopleEndpoint);
        }

        public async Task<IEnumerable<People>> GetPeopleInPositionsAsync(IEnumerable<Position> positions)
        {
            var url = new UriBuilder(Endpoints.PeopleEndpoint);
            var positionsString = positions.Select(_ => _.Id.ToString()).Aggregate((curr, next) => $"{curr},{next}");
            url.AddQuery("positions", positionsString);
            return await GetAsyncWrapper<IEnumerable<People>>(url.ToString());
        }

        public async Task<IEnumerable<Kpi>> GetFinancialKpiAsync(ContractDatum userContract)
        {
            var url = string.Format(Endpoints.KpiFinancialCustomAdminEndpoint, userContract.Id);
            return await GetAsyncWrapper<IEnumerable<Kpi>>(url);
        }

        public async Task<IEnumerable<Kpi>> GetNonFinancialKpiAsync(ContractDatum userContract)
        {
            var url = string.Format(Endpoints.KpiNonFinancialCustomAdminEndpoint, userContract.Id);
            return await GetAsyncWrapper<IEnumerable<Kpi>>(url);
        }

        public async Task<IEnumerable<Goal>> GetGoalsAsync(ContractDatum userContract, Roadmap roadmap)
        {
            var url = string.Format(Endpoints.GoalsAdminEndpoint, userContract.Id, roadmap.Year);
            return await GetAsyncWrapper<IEnumerable<Goal>>(url);
        }

        public async Task<Roadmaps> GetRoadmapsAsync(ContractDatum userContract)
        {
            var url = string.Format(Endpoints.RoadmapsAdminEndpoint, userContract.Id);
            return await GetAsyncWrapper<Roadmaps>(url);
        }

        public async Task<Profile> GetProfileByIdAsync(long Id)
        {
            return await GetAsyncWrapper<Profile>($"{Endpoints.ProfileEndpoint}{Id}");
        }

        public async Task<Kpi> CreateFinancialKpiAsync(ContractDatum userContract, Kpi kpi)
        {
            var url = string.Format(Endpoints.KpiFinancialCustomAdminEndpoint, userContract.Id);
            return await PostAsyncWrapper<Kpi, Kpi>(kpi, url);
        }

        public async Task<Kpi> CreateNonFinancialKpiAsync(ContractDatum userContract, Kpi kpi)
        {
            var url = string.Format(Endpoints.KpiNonFinancialCustomAdminEndpoint, userContract.Id);
            return await PostAsyncWrapper<Kpi, Kpi>(kpi, url);
        }

        public async Task<Kpi> UpdateFinancialKpiAsync(ContractDatum userContract, Kpi kpi)
        {
            var url = $"{string.Format(Endpoints.KpiFinancialCustomAdminEndpoint, userContract.Id)}{kpi.Id}/";
            return await PatchAsyncWrapper<Kpi, Kpi>(kpi, url);
        }

        public async Task<Kpi> UpdateNonFinancialKpiAsync(ContractDatum userContract, Kpi kpi)
        {
            var url = $"{string.Format(Endpoints.KpiNonFinancialCustomAdminEndpoint, userContract.Id)}{kpi.Id}/";
            return await PatchAsyncWrapper<Kpi, Kpi>(kpi, url);
        }

        public async Task<Roadmap> CreateRoadmapAsync(ContractDatum userContract, Roadmap roadmap)
        {
            var url = string.Format(Endpoints.RoadmapsAdminEndpoint, userContract.Id);
            return await PostAsyncWrapper<Roadmap, Roadmap>(roadmap, url);
        }

        public async Task<Goal> CreateGoalAsync(ContractDatum userContract, Roadmap roadmap, Goal goal)
        {
            var url = string.Format(Endpoints.GoalsAdminEndpoint, userContract.Id, roadmap.Year);
            return await PostAsyncWrapper<Goal, Goal>(goal, url);
        }

        public async Task<Goal> UpdateGoalAsync(ContractDatum userContract, Roadmap roadmap, Goal goal)
        {
            var url = $"{string.Format(Endpoints.GoalsAdminEndpoint, userContract.Id, roadmap.Year)}{goal.Id}/";
            return await PatchAsyncWrapper<Goal, Goal>(goal, url);
        }

        public async Task<bool> DeleteGoalAsync(ContractDatum userContract, Roadmap roadmap, Goal goal)
        {
            var url = $"{string.Format(Endpoints.GoalsAdminEndpoint, userContract.Id, roadmap.Year)}{goal.Id}/";
            return await DeleteAsyncWrapper(url);
        }

        public async Task<bool> DeleteFinancialKpiAsync(ContractDatum userContract, Kpi kpi)
        {
            var url = $"{string.Format(Endpoints.KpiFinancialCustomAdminEndpoint, userContract.Id)}{kpi.Id}/";
            return await DeleteAsyncWrapper(url);
        }

        public async Task<bool> DeleteNonFinancialKpiAsync(ContractDatum userContract, Kpi kpi)
        {
            var url = $"{string.Format(Endpoints.KpiNonFinancialCustomAdminEndpoint, userContract.Id)}{kpi.Id}/";
            return await DeleteAsyncWrapper(url);
        }

        public void Dispose()
        {
            AmSpaceHttpClient = null;
            IsAthorized = false;
            LoginResult = null;
            ClientId = null;
            GrantPermissionType = null;
            CookieContainer = null;
        }

        public async Task<IEnumerable<Brand>> GetBrandsAsync()
        {
            return await GetAsyncWrapper<IEnumerable<Brand>>(Endpoints.BrandsEndpoint);
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync(Brand brand)
        {
            var url = string.Format(Endpoints.CountriesEndpoint, brand.Id);
            return await GetAsyncWrapper<IEnumerable<Country>>(url);
        }

        public async Task<IEnumerable<SearchUserResult>> FindUser(string query, Brand brand, OrganizationGroup orgGroup, UserStatus status, string domain)
        {
            var url = string.Format(Endpoints.SearchUsersEndpoint, query, brand.Id, orgGroup.Id, status, domain);
            var pager = await GetAsyncWrapper<SearchUsers>(url);
            var result = new List<SearchUserResult>();
            result.AddRange(pager.Results);
            while (!string.IsNullOrEmpty(pager.Next))
            {
                pager = await GetAsyncWrapper<SearchUsers>(pager.Next);
                result.AddRange(pager.Results);
            }
            return result;
        }
    }
}
