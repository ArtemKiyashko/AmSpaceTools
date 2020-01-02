using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels
{
    public class AmSpaceSsoOptions
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string RedirectUrl { get; set; }
        public IEnumerable<string> Scopes { get; set; }
    }
}
