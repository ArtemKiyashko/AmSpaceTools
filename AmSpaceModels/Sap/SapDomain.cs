using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.Sap
{
    public class SapDomain
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("domain_id")]
        public int DomainId { get; set; }

        [JsonProperty("parent_domain_id")]
        public int? ParentDomainId { get; set; }

        [JsonProperty("mpk")]
        public int Mpk { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }
    }

}
