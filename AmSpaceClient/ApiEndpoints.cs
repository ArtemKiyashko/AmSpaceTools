using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceClient
{
    public class ApiEndpoints
    {
        public string TokenEndpoint { get { return $"{BaseAddress}/api/v1/auth/token/"; }  }
        public string ProfileEndpoint { get { return $"{BaseAddress}/api/v1/profile/"; } }
        public string CompetencyAdminEndpoint { get { return $"{BaseAddress}/api/v1/search/idp/admin/"; } }
        public string LevelsEndpoint { get { return $"{BaseAddress}/api/v1/organization/levels/"; } }
        public string CompetecyActionAdminEndpoint { get { return $"{BaseAddress}/api/v1/idp/admin/public/plans/{{0}}/"; } }
        public string UpdateActionEndpoint { get { return $"{BaseAddress}/api/v1/idp/admin/public/plans/{{0}}/batch/"; } }
        public string LogoutEndpoint { get { return $"{BaseAddress}/api/v1/o/revoke_token/"; } }
        public string UserSapEndpoint { get { return $"{BaseAddress}/api/v1/sap/sync/user/"; } }
        public string DomainSapEndpoint { get { return $"{BaseAddress}/api/v1/sap/sync/domain/"; } }
        public string UsersInDomainEndpoint { get { return $"{BaseAddress}/api/v1/organization/people/?domains="; } }
        public string DomainNodesEndpoint { get { return $"{BaseAddress}/api/v1/organization/tree/?with_mpk=1"; } }
        public string PositionsEndpoint { get { return $"{BaseAddress}/api/v1/organization/positions/"; } }
        public string PeopleEndpoint { get { return $"{BaseAddress}/api/v1/organization/people/"; } }
        /// <summary>
        /// <remarks>Contract ID</remarks>
        /// </summary>
        public string KpiFinancialCustomAdminEndpoint { get { return $"{BaseAddress}/api/v1/performance/kpi/admin/{{0}}/custom/financial/"; } }
        /// <summary>
        /// <remarks>Contract ID</remarks>
        /// </summary>
        public string KpiNonFinancialCustomAdminEndpoint { get { return $"{BaseAddress}/api/v1/performance/kpi/admin/{{0}}/custom/non_financial/"; } }
        /// <summary>
        /// <remarks>Contract ID</remarks>
        /// </summary>
        public string RoadmapsAdminEndpoint { get { return $"{BaseAddress}/api/v1/performance/goals/admin/{{0}}/roadmaps/"; } }
        /// <summary>
        /// <remarks>Contract ID, Year</remarks>
        /// </summary>
        public string RoadmapUpdateAdminEndpoint { get { return $"{BaseAddress}/api/v1/performance/goals/admin/{{0}}/roadmaps/{{1}}/"; } }
        /// <summary>
        /// <remarks>
        /// Contract ID, Year
        /// </remarks>
        /// </summary>
        public string GoalsAdminEndpoint { get { return $"{BaseAddress}/api/v1/performance/goals/admin/{{0}}/roadmaps/{{1}}/goals/"; } }
        public string BrandsEndpoint { get { return $"{BaseAddress}/api/v1/organization/brands/"; } }
        /// <summary>
        /// <remarks>Brand ID</remarks>
        /// </summary>
        public string CountriesEndpoint { get { return $"{BaseAddress}/api/v1/organization/brands/{{0}}/countries/"; } }
        /// <summary>
        /// <remarks>
        /// User name, Brand ID, Organizational group (crew, managers), User status (active, suspended, terminated), domain name (PL, CZ, etc), identity number (like passport number, unique)
        /// </remarks>
        /// </summary>
        public string SearchUsersEndpoint { get { return $"{BaseAddress}/api/v1/search/users/"; } }
        public string OrganizationGroupsEndpoint { get { return $"{BaseAddress}/api/v1/organization/organization_groups/"; } }
        /// <summary>
        /// <remarks>
        /// Contract ID, Year
        /// </remarks>
        /// </summary>
        public string GoalsWeightAdminEndpoint { get { return $"{BaseAddress}/api/v1/performance/goals/admin/{{0}}/roadmaps/{{1}}/goals/manager/"; } }
        public string TemporaryAccountAdminEndpoint { get { return $"{BaseAddress}/api/v1/accounts/create_temporary/"; } }
        /// <summary>
        /// Required query string parameters to add: page, query and domain for OPS or tag for RST
        /// </summary>
        public string JobMapSearchEndpoint { get { return $"{BaseAddress}/api/v1/search/organization/jobs/?query={{0}}&level={{1}}&tag={{2}}&domain={{3}}"; } }
        /// <summary>
        /// PATCH only
        /// </summary>
        public string JobDescriptionEndpoint { get { return $"{BaseAddress}/api/v1/organization/jobs/{{0}}/description/"; } }
        public string JobResponsibilitiesEndpoint { get { return $"{BaseAddress}/api/v1/organization/jobs/{{0}}/responsibilities/"; } }
        public string ExternalAccountCreateEndpoint { get { return $"{BaseAddress}/api/v1/accounts/external/"; } }
        /// <summary>
        /// <remarks>
        /// ContractId
        /// </remarks>
        /// </summary>
        public string ExternalAccountUpdateEndpoint { get { return $"{BaseAddress}/api/v1/accounts/external/{{0}}/"; } }

        /// <summary>
        /// <remarks>
        /// BrandId (in case of RST = "rst");
        /// </remarks>
        /// </summary>
        public string BrandLevelsEndpoint { get { return $"{BaseAddress}/api/v1/organization/domains/{{0}}/levels/"; } }

        /// <summary>
        /// <remarks>
        /// BrandId (in case of RST = "rst"); LevelId
        /// </remarks>
        /// </summary>
        public string CompetencielsModelEndpoint { get { return $"{BaseAddress}/api/v1/organization/domains/{{0}}/levels/{{1}}/level_competencies/"; } }

        /// <summary>
        /// <remarks>
        /// BrandId (in case of RST = "rst"); LevelId
        /// </remarks>
        /// </summary>
        public string CoreValuesEndpoint { get { return $"{BaseAddress}/api/v1/organization/domains/{{0}}/levels/{{1}}/core_values/"; } }

        public string DefaultAvatarEndpoint { get { return $"{BaseAddress}/static/avatar.png"; } }

        /// <summary>
        /// Change password for your subbordinate
        /// </summary>
        public string ChangePasswordTeamEndpoint { get { return $"{BaseAddress}/api/v1/team/user/{{0}}/change_password/"; } }

        /// <summary>
        /// Change password for user by UserId
        /// </summary>
        public string ChangePasswordEndpoint { get { return $"{BaseAddress}/api/v1/auth/change_password/{{0}}/"; } }

        public string JpaHistoryAdminEndpoint { get { return $"{BaseAddress}/api/v1/jpa/forms/admin/history_user_forms/"; } }
        public string JpaHistoryUpdateAdminEndpoint { get { return $"{BaseAddress}/api/v1/jpa/forms/admin/history_user_forms/{{0}}/"; } }

        public string BaseAddress { get; private set; }

        public ApiEndpoints(string baseAddress)
        {
            BaseAddress = baseAddress;
        }
    }
}
