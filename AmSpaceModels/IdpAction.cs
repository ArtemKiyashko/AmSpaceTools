using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels
{
    public partial class IdpAction
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("action_type")]
        public ActionType ActionType { get; set; }

        [JsonProperty("translations")]
        public List<Translation> Translations { get; set; }

        [JsonProperty("editable")]
        public bool Editable { get; set; }

        [JsonIgnore]
        public bool Updated { get; set; }
    }
}
