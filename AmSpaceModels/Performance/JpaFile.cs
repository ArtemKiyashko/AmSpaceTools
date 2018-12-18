using System;
using Newtonsoft.Json;

namespace AmSpaceModels.Performance
{
    public partial class JpaFile
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("jpa_name")]
        public string JpaName { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("file")]
        public Uri File { get; set; }

        [JsonProperty("user")]
        public string UserName { get; set; }
    }
}