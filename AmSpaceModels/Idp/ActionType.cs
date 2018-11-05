using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.Idp
{
    public partial class ActionType
    {
        [JsonProperty("value")]
        public long Value { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
    }
}
