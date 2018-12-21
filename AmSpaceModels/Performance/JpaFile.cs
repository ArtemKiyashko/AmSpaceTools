using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AmSpaceModels.Performance
{
    public partial class JpaFileList
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("previous")]
        public string Previous { get; set; }

        [JsonProperty("results")]
        public IEnumerable<JpaFile> Results { get; set; }
    }
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