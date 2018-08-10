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
        public string DomainNodesEndpoint { get { return $"{BaseAddress}/api/v1/organization/tree/"; } }
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
        /// <remarks>
        /// Contract ID, Year
        /// </remarks>
        /// </summary>
        public string GoalsAdminEndpoint { get { return $"{BaseAddress}/api/v1/performance/goals/admin/{{0}}/roadmaps/{{1}}/goals/"; } }

        public string BaseAddress { get; private set; }

        public ApiEndpoints(string baseAddress)
        {
            BaseAddress = baseAddress;
        }
    }
}
