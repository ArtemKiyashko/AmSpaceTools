using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels
{
    public class ApiEndpoits
    {
        public Uri TokenEndpoint {  get;  } = new Uri("api/v1/auth/token/");
        public Uri ProfileEndpoint { get; } = new Uri("api/v1/profile/");
        public Uri CompetencyEndpoint { get; } = new Uri("api/v1/search/idp/admin/");
        public Uri LevelsEndpoint { get; } = new Uri("api/v1/organization/levels/");
        public Uri CompetecyEndpoint { get; } = new Uri("api/v1/idp/admin/public/plans/{0}/");
        public Uri UpdateActionEndpoint { get; } = new Uri("api/v1/idp/admin/public/plans/{0}/batch/");
    }
}
