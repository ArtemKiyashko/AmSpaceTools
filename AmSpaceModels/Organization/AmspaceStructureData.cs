using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.Organization
{
    public class AmspaceDomain
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mpk")]
        public long Mpk { get; set; }

        [JsonProperty("children")]
        public IEnumerable<AmspaceDomain> Children { get; set; }
    }

    public class AmspaceUser
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

}
