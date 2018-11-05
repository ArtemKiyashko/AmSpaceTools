using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.JobMap
{
    public class JobDescription
    {
        [JsonProperty("id")]
        public long? Id { get; set; }
        [JsonProperty("translations")]
        public IEnumerable<JobDescriptionTranslation> Translations { get; set; }

    }

    public class JobDescriptionTranslation
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("additional_requirements")]
        public string AdditionalRequirements { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }
    }
}
