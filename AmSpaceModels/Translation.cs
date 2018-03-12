using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels
{
    public partial class Translation
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }
    }
}
