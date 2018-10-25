﻿using AmSpaceModels;
using AmSpaceModels.Enums;
using AmSpaceModels.Idp;
using AmSpaceModels.JobMap;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UriBuilderExtended;

namespace AmSpaceClient
{
    public class AmSpaceHttpClient : IAmSpaceClient
    {
        private IRequestWrapper _requestWrapper;

        public bool IsAthorized { get; private set; }
        public LoginResult LoginResult { get; private set; }
        public string ClientId { get; private set; }
        public string GrantPermissionType { get; private set; }
        public virtual ApiEndpoints Endpoints { get; private set; }
        public virtual IRequestWrapper RequestWrapper
        {
            get
            {
                if (_requestWrapper == null)
                    _requestWrapper = new RequestWrapper();
                return _requestWrapper;
            }
        }

        public AmSpaceHttpClient()
        {
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
            LoginResult = await RequestWrapper.PostAsyncWrapper<LoginResult>(Endpoints.TokenEndpoint, content);
            RequestWrapper.AddAuthHeaders(new AuthenticationHeaderValue("Bearer", LoginResult.AccessToken));
            RequestWrapper.AddAuthCookies(new Uri(Endpoints.BaseAddress), new Cookie("accessToken", LoginResult.AccessToken));
            IsAthorized = true;
            return true;
        }

        public async Task<BitmapSource> GetAvatarAsync(string link)
        {
            var result = await RequestWrapper.GetAsyncWrapper(link);
            if (!result.IsSuccessStatusCode)
                result = await RequestWrapper.GetAsyncWrapper($"{Endpoints.BaseAddress}/static/avatar.png");
            await result.ValidateAsync();
            var content = await result.Content.ReadAsByteArrayAsync();
            return (BitmapSource)new ImageSourceConverter().ConvertFrom(content);
        }

        public async Task<IEnumerable<Level>> GetLevelsAsync()
        {
            return await RequestWrapper.GetAsyncWrapper<IEnumerable<Level>>(Endpoints.LevelsEndpoint);
        }

        public async Task<IEnumerable<Competency>> GetCompetenciesAsync()
        {
            var pager = await RequestWrapper.GetAsyncWrapper<CompetencyPager>(Endpoints.CompetencyAdminEndpoint);
            var allComps = new List<Competency>();
            allComps.AddRange(pager.Results);
            while (!string.IsNullOrEmpty(pager.Next))
            {
                pager = await RequestWrapper.GetAsyncWrapper<CompetencyPager>(pager.Next);
                allComps.AddRange(pager.Results);
            }
            var levels = await GetLevelsAsync();
            allComps.ForEach(comp => comp.Level = levels.FirstOrDefault(_ => _.Id == comp.LevelId));
            return allComps;
        }

        public async Task<CompetencyAction> GetCompetencyActionsAsync(long competencyId)
        {
            return await RequestWrapper.GetAsyncWrapper<CompetencyAction>(string.Format(Endpoints.CompetecyActionAdminEndpoint, competencyId.ToString()));
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
            var result = await RequestWrapper.PostAsyncWrapper(Endpoints.LogoutEndpoint, content);
            await result.ValidateAsync();
            IsAthorized = false;
            return true;
        }

        public async Task<Profile> GetProfileAsync()
        {
            return await RequestWrapper.GetAsyncWrapper<Profile>(Endpoints.ProfileEndpoint);
        }

        public async Task<bool> UpdateActionAsync(UpdateAction model, long competencyId)
        {
            var endpoint = string.Format(Endpoints.UpdateActionEndpoint, competencyId.ToString());
            return await RequestWrapper.PutAsyncWrapper(model, endpoint);
        }

        public async Task<IEnumerable<AmspaceDomain>> GetOrganizationStructureAsync()
        {
            return await RequestWrapper.GetAsyncWrapper<IEnumerable<AmspaceDomain>>(Endpoints.DomainNodesEndpoint);
        }

        public async Task<IEnumerable<AmspaceUser>> GetDomainUsersAsync(int domainId)
        {
            return await RequestWrapper.GetAsyncWrapper<IEnumerable<AmspaceUser>>(Endpoints.UsersInDomainEndpoint + domainId);
        }

        public async Task<bool> PutUserAsync(SapUser user)
        {
            return await RequestWrapper.PutAsyncWrapper(user, Endpoints.UserSapEndpoint);
        }

        public async Task<bool> PutDomainAsync(SapDomain domain)
        {
            return await RequestWrapper.PutAsyncWrapper(domain, Endpoints.DomainSapEndpoint);
        }

        public async Task<bool> DisableUserAsync(SapUserDelete user)
        {
            return await RequestWrapper.DeleteAsyncWrapper(user, Endpoints.UserSapEndpoint);
        }

        public async Task<IEnumerable<Position>> GetPositionsInLevelsAsync(IEnumerable<Level> levels)
        {
            var url = new UriBuilder(Endpoints.PositionsEndpoint);
            var levelsString = levels.Select(_ => _.Id.ToString()).Aggregate((curr, next) => $"{curr},{next}");
            url.AddQuery("levels", levelsString);
            return await RequestWrapper.GetAsyncWrapper<IEnumerable<Position>>(url.ToString());
        }

        public async Task<IEnumerable<Position>> GetPositionsAsync()
        {
            return await RequestWrapper.GetAsyncWrapper<IEnumerable<Position>>(Endpoints.PositionsEndpoint);
        }

        public async Task<IEnumerable<People>> GetPeopleAsync()
        {
            return await RequestWrapper.GetAsyncWrapper<IEnumerable<People>>(Endpoints.PeopleEndpoint);
        }

        public async Task<IEnumerable<People>> GetPeopleInPositionsAsync(IEnumerable<Position> positions)
        {
            var url = new UriBuilder(Endpoints.PeopleEndpoint);
            var positionsString = positions.Select(_ => _.Id.ToString()).Aggregate((curr, next) => $"{curr},{next}");
            url.AddQuery("positions", positionsString);
            return await RequestWrapper.GetAsyncWrapper<IEnumerable<People>>(url.ToString());
        }

        public async Task<IEnumerable<Kpi>> GetFinancialKpiAsync(ContractSearch userContract)
        {
            var url = string.Format(Endpoints.KpiFinancialCustomAdminEndpoint, userContract.Id);
            return await RequestWrapper.GetAsyncWrapper<IEnumerable<Kpi>>(url);
        }

        public async Task<IEnumerable<Kpi>> GetNonFinancialKpiAsync(ContractSearch userContract)
        {
            var url = string.Format(Endpoints.KpiNonFinancialCustomAdminEndpoint, userContract.Id);
            return await RequestWrapper.GetAsyncWrapper<IEnumerable<Kpi>>(url);
        }

        public async Task<IEnumerable<Goal>> GetGoalsAsync(ContractSearch userContract, Roadmap roadmap)
        {
            var url = string.Format(Endpoints.GoalsAdminEndpoint, userContract.Id, roadmap.Year);
            return await RequestWrapper.GetAsyncWrapper<IEnumerable<Goal>>(url);
        }

        public async Task<Roadmaps> GetRoadmapsAsync(ContractSearch userContract)
        {
            var url = string.Format(Endpoints.RoadmapsAdminEndpoint, userContract.Id);
            return await RequestWrapper.GetAsyncWrapper<Roadmaps>(url);
        }

        public async Task<Profile> GetProfileByIdAsync(long Id)
        {
            return await RequestWrapper.GetAsyncWrapper<Profile>($"{Endpoints.ProfileEndpoint}{Id}");
        }

        public async Task<Kpi> CreateFinancialKpiAsync(ContractSearch userContract, Kpi kpi)
        {
            var url = string.Format(Endpoints.KpiFinancialCustomAdminEndpoint, userContract.Id);
            return await RequestWrapper.PostAsyncWrapper<Kpi, Kpi>(kpi, url);
        }

        public async Task<Kpi> CreateNonFinancialKpiAsync(ContractSearch userContract, Kpi kpi)
        {
            var url = string.Format(Endpoints.KpiNonFinancialCustomAdminEndpoint, userContract.Id);
            return await RequestWrapper.PostAsyncWrapper<Kpi, Kpi>(kpi, url);
        }

        public async Task<Kpi> UpdateFinancialKpiAsync(ContractSearch userContract, Kpi kpi)
        {
            var url = $"{string.Format(Endpoints.KpiFinancialCustomAdminEndpoint, userContract.Id)}{kpi.Id}/";
            return await RequestWrapper.PatchAsyncWrapper<Kpi, Kpi>(kpi, url);
        }

        public async Task<Kpi> UpdateNonFinancialKpiAsync(ContractSearch userContract, Kpi kpi)
        {
            var url = $"{string.Format(Endpoints.KpiNonFinancialCustomAdminEndpoint, userContract.Id)}{kpi.Id}/";
            return await RequestWrapper.PatchAsyncWrapper<Kpi, Kpi>(kpi, url);
        }

        public async Task<Roadmap> CreateRoadmapAsync(ContractSearch userContract, Roadmap roadmap)
        {
            var url = string.Format(Endpoints.RoadmapsAdminEndpoint, userContract.Id);
            return await RequestWrapper.PostAsyncWrapper<Roadmap, Roadmap>(roadmap, url);
        }

        public async Task<Goal> CreateGoalAsync(ContractSearch userContract, Roadmap roadmap, GoalNew goal)
        {
            var url = string.Format(Endpoints.GoalsAdminEndpoint, userContract.Id, roadmap.Year);
            return await RequestWrapper.PostAsyncWrapper<GoalNew, Goal>(goal, url);
        }

        public async Task<Goal> UpdateGoalAsync(ContractSearch userContract, Roadmap roadmap, Goal goal)
        {
            var url = $"{string.Format(Endpoints.GoalsAdminEndpoint, userContract.Id, roadmap.Year)}{goal.Id}/";
            return await RequestWrapper.PatchAsyncWrapper<Goal, Goal>(goal, url);
        }

        public async Task<bool> DeleteGoalAsync(ContractSearch userContract, Roadmap roadmap, Goal goal)
        {
            var url = $"{string.Format(Endpoints.GoalsAdminEndpoint, userContract.Id, roadmap.Year)}{goal.Id}/";
            return await RequestWrapper.DeleteAsyncWrapper(url);
        }

        public async Task<bool> DeleteFinancialKpiAsync(ContractSearch userContract, Kpi kpi)
        {
            var url = $"{string.Format(Endpoints.KpiFinancialCustomAdminEndpoint, userContract.Id)}{kpi.Id}/";
            return await RequestWrapper.DeleteAsyncWrapper(url);
        }

        public async Task<bool> DeleteNonFinancialKpiAsync(ContractSearch userContract, Kpi kpi)
        {
            var url = $"{string.Format(Endpoints.KpiNonFinancialCustomAdminEndpoint, userContract.Id)}{kpi.Id}/";
            return await RequestWrapper.DeleteAsyncWrapper(url);
        }

        public void Dispose()
        {
            _requestWrapper = null;
            IsAthorized = false;
            LoginResult = null;
            ClientId = null;
            GrantPermissionType = null;
        }

        public async Task<IEnumerable<Brand>> GetBrandsAsync()
        {
            return await RequestWrapper.GetAsyncWrapper<IEnumerable<Brand>>(Endpoints.BrandsEndpoint);
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync(Brand brand)
        {
            var url = string.Format(Endpoints.CountriesEndpoint, brand.Id);
            return await RequestWrapper.GetAsyncWrapper<IEnumerable<Country>>(url);
        }

        public async Task<IEnumerable<SearchUserResult>> FindUser(string query, Brand brand, OrganizationGroup orgGroup, UserStatus status, string domain)
        {
            var url = string.Format(Endpoints.SearchUsersEndpoint, query, brand?.Id, orgGroup?.Id, status == UserStatus.ANY ? (object)string.Empty : (int)status, domain);
            var pager = await RequestWrapper.GetAsyncWrapper<SearchUsers>(url);
            var result = new List<SearchUserResult>();
            result.AddRange(pager.Results);
            while (!string.IsNullOrEmpty(pager.Next))
            {
                pager = await RequestWrapper.GetAsyncWrapper<SearchUsers>(pager.Next);
                result.AddRange(pager.Results);
            }
            return result;
        }

        public async Task<IEnumerable<OrganizationGroup>> GetOrganizationGroupsAsync()
        {
            return await RequestWrapper.GetAsyncWrapper<IEnumerable<OrganizationGroup>>(Endpoints.OrganizationGroupsEndpoint);
        }

        public async Task<IEnumerable<GoalWeight>> UpdateGoalsWeight(IEnumerable<GoalWeight> weights, ContractSearch userContract, Roadmap roadmap)
        {
            var url = string.Format(Endpoints.GoalsWeightAdminEndpoint, userContract.Id, roadmap.Year);
            return await RequestWrapper.PatchAsyncWrapper<IEnumerable<GoalWeight>, IEnumerable<GoalWeight>>(weights, url);
        }

        public async Task<Roadmap> UpdateRoadmapAsync(ContractSearch userContract, Roadmap roadmap)
        {
            var url = string.Format(Endpoints.RoadmapUpdateAdminEndpoint, userContract.Id, roadmap.Year);
            roadmap.Year = null;
            return await RequestWrapper.PatchAsyncWrapper<Roadmap, Roadmap>(roadmap, url);
        }

        public async Task<TemporaryAccount> CreateTemporaryAccount(TemporaryAccount accountInfo)
        {
            return await RequestWrapper.PostAsyncWrapper<TemporaryAccount, TemporaryAccount>(accountInfo, Endpoints.TemporaryAccountAdminEndpoint);
        }

        public Task<ExternalAccount> CreateExternalAccount(ExternalAccount accountInfo)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<JobMap>> FindJobMap(string country, string brand, int level, string position)
        {
            var queryString = await BuildJobMapQueryString(country, brand, level, position);
            var pager = await RequestWrapper.GetAsyncWrapper<JobSearchPager>($"{Endpoints.JobMapSearchEndpoint}{queryString}");
            var result = new List<JobMap>();
            result.AddRange(pager.Results);
            while (!string.IsNullOrEmpty(pager.Next))
            {
                pager = await RequestWrapper.GetAsyncWrapper<JobSearchPager>(pager.Next);
                result.AddRange(pager.Results);
            }
            return result;
        }

        private async Task<string> BuildJobMapQueryString(string country, string brand, int level, string position)
        {
            var queryString = $"?query={position}&level={level}&";
            var currentBrand = (await GetBrandsAsync()).FirstOrDefault(item => item.Name == brand);
            if (currentBrand == null)
            {
                queryString = queryString + "tag=rst";
            }
            else
            {
                var countries = await GetCountriesAsync(currentBrand);
                var currentCountry = countries.FirstOrDefault(item => item.Name.Contains(country));
                queryString = $"{queryString}domain={currentCountry.Id}";
            }
            return queryString;
        }

        public async Task<JobDescription> UpdateJobDescriptionAsync(JobDescription jobDescription)
        {
            var url = string.Format(Endpoints.JobDescriptionEndpoint, jobDescription.Id);
            return await RequestWrapper.PatchAsyncWrapper<JobDescription, JobDescription>(jobDescription, url);
        }

        public async Task<List<JobResponsibility>> GetJobResponsibilities(JobMap jobMap)
        {
            var url = string.Format(Endpoints.JobResponsibilitiesEndpoint, jobMap.Id);
            return await RequestWrapper.GetAsyncWrapper<List<JobResponsibility>>(url);
        }

        public async Task<bool> DeleteJobResponsibility(JobResponsibility responsibility)
        {
            var url = string.Format(Endpoints.JobResponsibilitiesEndpoint, responsibility.Job) + $"{responsibility.Id}/";
            return await RequestWrapper.DeleteAsyncWrapper(url);
        }

        public async Task<JobResponsibility> CreateJobResponsibility(JobResponsibility responsibility)
        {
            var url = string.Format(Endpoints.JobResponsibilitiesEndpoint, responsibility.Job);
            return await RequestWrapper.PostAsyncWrapper<JobResponsibility, JobResponsibility>(responsibility, url);
        }

    }
}
