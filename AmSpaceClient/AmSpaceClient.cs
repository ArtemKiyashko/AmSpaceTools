using AmSpaceModels;
using AmSpaceModels.Idp;
using AmSpaceModels.Organization;
using AmSpaceModels.Performance;
using AmSpaceModels.Sap;
using AmSpaceTools.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UriBuilderExtended;

namespace AmSpaceClient
{
    public class AmSpaceClient : IAmSpaceClient
    {
        
        public bool IsAthorized { get; private set; }
        public LoginResult LoginResult { get; private set; }
        public string ClientId { get; private set; }
        public string GrantPermissionType { get; private set; }
        public virtual ApiEndpoints Endpoints { get; private set; }
        public virtual IRequestsWrapper RequestsWrapper { get; private set; }
        
        
        public AmSpaceClient()
        {
            RequestsWrapper = new RequestsWrapper();
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
            LoginResult = await RequestsWrapper.PostAsyncWrapper<LoginResult>(Endpoints.TokenEndpoint, content);
            RequestsWrapper.AddAuthHeaders(new AuthenticationHeaderValue("Bearer", LoginResult.AccessToken));
            RequestsWrapper.AddAuthCookies(new Uri(Endpoints.BaseAddress), new Cookie("accessToken", LoginResult.AccessToken));
            IsAthorized = true;
            return true;
        }

        public async Task<BitmapSource> GetAvatarAsync(string link, IImageConverter converter = null)
        {
            //  if (!IsAthorized) throw new UnauthorizedAccessException();
            var result = await RequestsWrapper.GetAsyncWrapper(link);
            if (!result.IsSuccessStatusCode)
                result = await RequestsWrapper.GetAsyncWrapper($"{Endpoints.BaseAddress}/static/avatar.png");
            await result.ValidateAsync();
            var content = await result.Content.ReadAsByteArrayAsync();
            return (converter ?? new BitmapSourceConverter()).ConvertFromByteArray(content);
        }

        public async Task<IEnumerable<Competency>> GetCompetenciesAsync()
        {
            var pager = await RequestsWrapper.GetAsyncWrapper<CompetencyPager>(Endpoints.CompetencyAdminEndpoint);
            var allComps = new List<Competency>();
            allComps.AddRange(pager.Results);
            while (!string.IsNullOrEmpty(pager.Next))
            {
                pager = await RequestsWrapper.GetAsyncWrapper<CompetencyPager>(pager.Next);
                allComps.AddRange(pager.Results);
            }
            var levels = await GetLevelsAsync();
            allComps.ForEach(comp => comp.Level = levels.FirstOrDefault(_ => _.Id == comp.LevelId));
            return allComps;
        }

        public async Task<CompetencyAction> GetCompetencyActionsAsync(long competencyId)
        {
            return await RequestsWrapper.GetAsyncWrapper<CompetencyAction>(string.Format(Endpoints.CompetecyActionAdminEndpoint, competencyId.ToString()));
        }

        public async Task<IEnumerable<Level>> GetLevelsAsync()
        {
            return await RequestsWrapper.GetAsyncWrapper<IEnumerable<Level>>(Endpoints.LevelsEndpoint);
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
            var result = await RequestsWrapper.PostAsyncWrapper(Endpoints.LogoutEndpoint, content);
            await result.ValidateAsync();
            IsAthorized = false;
            return true;
        }

        public async Task<Profile> GetProfileAsync()
        {
            return await RequestsWrapper.GetAsyncWrapper<Profile>(Endpoints.ProfileEndpoint);
        }

        public async Task<bool> UpdateActionAsync(UpdateAction model, long competencyId)
        {
            var endpoint = string.Format(Endpoints.UpdateActionEndpoint, competencyId.ToString());
            return await RequestsWrapper.PutAsyncWrapper(model, endpoint);
        }

        public async Task<IEnumerable<AmspaceDomain>> GetOrganizationStructureAsync()
        {
            return await RequestsWrapper.GetAsyncWrapper<IEnumerable<AmspaceDomain>>(Endpoints.DomainNodesEndpoint);
        }

        public async Task<IEnumerable<AmspaceUser>> GetDomainUsersAsync(int domainId)
        {
            return await RequestsWrapper.GetAsyncWrapper<IEnumerable<AmspaceUser>>(Endpoints.UsersInDomainEndpoint + domainId);
        }

        public async Task<bool> PutUserAsync(SapUser user)
        {
            return await RequestsWrapper.PutAsyncWrapper(user, Endpoints.UserSapEndpoint);
        }

        public async Task<bool> PutDomainAsync(SapDomain domain)
        {
            return await RequestsWrapper.PutAsyncWrapper(domain, Endpoints.DomainSapEndpoint);
        }

        public async Task<bool> DisableUserAsync(SapUserDelete user)
        {
            return await RequestsWrapper.DeleteAsyncWrapper(user, Endpoints.UserSapEndpoint);
        }

        public async Task<IEnumerable<Position>> GetPositionsInLevelsAsync(IEnumerable<Level> levels)
        {
            var url = new UriBuilder(Endpoints.PositionsEndpoint);
            var levelsString = levels.Select(_ => _.Id.ToString()).Aggregate((curr, next) => $"{curr},{next}");
            url.AddQuery("levels", levelsString);
            return await RequestsWrapper.GetAsyncWrapper<IEnumerable<Position>>(url.ToString());
        }

        public async Task<IEnumerable<Position>> GetPositionsAsync()
        {
            return await RequestsWrapper.GetAsyncWrapper<IEnumerable<Position>>(Endpoints.PositionsEndpoint);
        }

        public async Task<IEnumerable<People>> GetPeopleAsync()
        {
            return await RequestsWrapper.GetAsyncWrapper<IEnumerable<People>>(Endpoints.PeopleEndpoint);
        }

        public async Task<IEnumerable<People>> GetPeopleInPositionsAsync(IEnumerable<Position> positions)
        {
            var url = new UriBuilder(Endpoints.PeopleEndpoint);
            var positionsString = positions.Select(_ => _.Id.ToString()).Aggregate((curr, next) => $"{curr},{next}");
            url.AddQuery("positions", positionsString);
            return await RequestsWrapper.GetAsyncWrapper<IEnumerable<People>>(url.ToString());
        }

        public async Task<IEnumerable<Kpi>> GetFinancialKpiAsync(ContractSearch userContract)
        {
            var url = string.Format(Endpoints.KpiFinancialCustomAdminEndpoint, userContract.Id);
            return await RequestsWrapper.GetAsyncWrapper<IEnumerable<Kpi>>(url);
        }

        public async Task<IEnumerable<Kpi>> GetNonFinancialKpiAsync(ContractSearch userContract)
        {
            var url = string.Format(Endpoints.KpiNonFinancialCustomAdminEndpoint, userContract.Id);
            return await RequestsWrapper.GetAsyncWrapper<IEnumerable<Kpi>>(url);
        }

        public async Task<IEnumerable<Goal>> GetGoalsAsync(ContractSearch userContract, Roadmap roadmap)
        {
            var url = string.Format(Endpoints.GoalsAdminEndpoint, userContract.Id, roadmap.Year);
            return await RequestsWrapper.GetAsyncWrapper<IEnumerable<Goal>>(url);
        }

        public async Task<Roadmaps> GetRoadmapsAsync(ContractSearch userContract)
        {
            var url = string.Format(Endpoints.RoadmapsAdminEndpoint, userContract.Id);
            return await RequestsWrapper.GetAsyncWrapper<Roadmaps>(url);
        }

        public async Task<Profile> GetProfileByIdAsync(long Id)
        {
            return await RequestsWrapper.GetAsyncWrapper<Profile>($"{Endpoints.ProfileEndpoint}{Id}");
        }

        public async Task<Kpi> CreateFinancialKpiAsync(ContractSearch userContract, Kpi kpi)
        {
            var url = string.Format(Endpoints.KpiFinancialCustomAdminEndpoint, userContract.Id);
            return await RequestsWrapper.PostAsyncWrapper<Kpi, Kpi>(kpi, url);
        }

        public async Task<Kpi> CreateNonFinancialKpiAsync(ContractSearch userContract, Kpi kpi)
        {
            var url = string.Format(Endpoints.KpiNonFinancialCustomAdminEndpoint, userContract.Id);
            return await RequestsWrapper.PostAsyncWrapper<Kpi, Kpi>(kpi, url);
        }

        public async Task<Kpi> UpdateFinancialKpiAsync(ContractSearch userContract, Kpi kpi)
        {
            var url = $"{string.Format(Endpoints.KpiFinancialCustomAdminEndpoint, userContract.Id)}{kpi.Id}/";
            return await RequestsWrapper.PatchAsyncWrapper<Kpi, Kpi>(kpi, url);
        }

        public async Task<Kpi> UpdateNonFinancialKpiAsync(ContractSearch userContract, Kpi kpi)
        {
            var url = $"{string.Format(Endpoints.KpiNonFinancialCustomAdminEndpoint, userContract.Id)}{kpi.Id}/";
            return await RequestsWrapper.PatchAsyncWrapper<Kpi, Kpi>(kpi, url);
        }

        public async Task<Roadmap> CreateRoadmapAsync(ContractSearch userContract, Roadmap roadmap)
        {
            var url = string.Format(Endpoints.RoadmapsAdminEndpoint, userContract.Id);
            return await RequestsWrapper.PostAsyncWrapper<Roadmap, Roadmap>(roadmap, url);
        }

        public async Task<Goal> CreateGoalAsync(ContractSearch userContract, Roadmap roadmap, Goal goal)
        {
            var url = string.Format(Endpoints.GoalsAdminEndpoint, userContract.Id, roadmap.Year);
            return await RequestsWrapper.PostAsyncWrapper<Goal, Goal>(goal, url);
        }

        public async Task<Goal> UpdateGoalAsync(ContractSearch userContract, Roadmap roadmap, Goal goal)
        {
            var url = $"{string.Format(Endpoints.GoalsAdminEndpoint, userContract.Id, roadmap.Year)}{goal.Id}/";
            return await RequestsWrapper.PatchAsyncWrapper<Goal, Goal>(goal, url);
        }

        public async Task<bool> DeleteGoalAsync(ContractSearch userContract, Roadmap roadmap, Goal goal)
        {
            var url = $"{string.Format(Endpoints.GoalsAdminEndpoint, userContract.Id, roadmap.Year)}{goal.Id}/";
            return await RequestsWrapper.DeleteAsyncWrapper(url);
        }

        public async Task<bool> DeleteFinancialKpiAsync(ContractSearch userContract, Kpi kpi)
        {
            var url = $"{string.Format(Endpoints.KpiFinancialCustomAdminEndpoint, userContract.Id)}{kpi.Id}/";
            return await RequestsWrapper.DeleteAsyncWrapper(url);
        }

        public async Task<bool> DeleteNonFinancialKpiAsync(ContractSearch userContract, Kpi kpi)
        {
            var url = $"{string.Format(Endpoints.KpiNonFinancialCustomAdminEndpoint, userContract.Id)}{kpi.Id}/";
            return await RequestsWrapper.DeleteAsyncWrapper(url);
        }

        public void Dispose()
        {
            RequestsWrapper = null;
            IsAthorized = false;
            LoginResult = null;
            ClientId = null;
            GrantPermissionType = null;
        }

        public async Task<IEnumerable<Brand>> GetBrandsAsync()
        {
            return await RequestsWrapper.GetAsyncWrapper<IEnumerable<Brand>>(Endpoints.BrandsEndpoint);
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync(Brand brand)
        {
            var url = string.Format(Endpoints.CountriesEndpoint, brand.Id);
            return await RequestsWrapper.GetAsyncWrapper<IEnumerable<Country>>(url);
        }

        public async Task<IEnumerable<SearchUserResult>> FindUser(string query, Brand brand, OrganizationGroup orgGroup, UserStatus status, string domain)
        {
            var url = string.Format(Endpoints.SearchUsersEndpoint, query, brand.Id, orgGroup.Id, (int)status, domain);
            var pager = await RequestsWrapper.GetAsyncWrapper<SearchUsers>(url);
            var result = new List<SearchUserResult>();
            result.AddRange(pager.Results);
            while (!string.IsNullOrEmpty(pager.Next))
            {
                pager = await RequestsWrapper.GetAsyncWrapper<SearchUsers>(pager.Next);
                result.AddRange(pager.Results);
            }
            return result;
        }

        public async Task<IEnumerable<OrganizationGroup>> GetOrganizationGroupsAsync()
        {
            return await RequestsWrapper.GetAsyncWrapper<IEnumerable<OrganizationGroup>>(Endpoints.OrganizationGroupsEndpoint);
        }
    }
}
