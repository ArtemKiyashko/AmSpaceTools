using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceClient
{
    public class ApiEndpoints
    {
        public string TokenEndpoint {  get;  } = "/api/v1/auth/token/";
        public string ProfileEndpoint { get; } = "/api/v1/profile/";
        public string CompetencyEndpoint { get; } = "/api/v1/search/idp/admin/";
        public string LevelsEndpoint { get; } = "/api/v1/organization/levels/";
        public string CompetecyEndpoint { get; } = "/api/v1/idp/admin/public/plans/{0}/";
        public string UpdateActionEndpoint { get; } = "/api/v1/idp/admin/public/plans/{0}/batch/";
    }
}
