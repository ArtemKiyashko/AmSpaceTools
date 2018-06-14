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
        public string CompetencyEndpoint { get { return $"{BaseAddress}/api/v1/search/idp/admin/"; } }
        public string LevelsEndpoint { get { return $"{BaseAddress}/api/v1/organization/levels/"; } }
        public string CompetecyActionEndpoint { get { return string.Format("{0}/api/v1/idp/admin/public/plans/{{0}}/", BaseAddress); } }
        public string UpdateActionEndpoint { get { return string.Format("{0}/api/v1/idp/admin/public/plans/{{0}}/batch/", BaseAddress); } }
        public string LogoutEndpoint { get { return $"{BaseAddress}/api/v1/o/revoke_token/"; } }
        public string UserSapEndpoint { get { return $"{BaseAddress}/api/v1/sap/sync/user/"; } }
        public string DomainSapEndpoint { get { return $"{BaseAddress}/api/v1/sap/sync/domain/"; } }
        public string UsersInDomainEndpoint { get { return $"{BaseAddress}/api/v1/organization/people/?domains="; } }
        public string DomainNodesEndpoint { get { return $"{BaseAddress}/api/v1/organization/tree/"; } }

        public string BaseAddress { get; private set; }

        public ApiEndpoints(string baseAddress)
        {
            BaseAddress = baseAddress;
        }
    }
}
