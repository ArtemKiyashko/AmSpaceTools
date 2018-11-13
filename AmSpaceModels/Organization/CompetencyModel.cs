using AmSpaceModels.Idp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.Organization
{
    public partial class Competencies
    {
        [JsonProperty("competencies")]
        public ICollection<CompetencyModel> CompetenciesList { get; set; }
    }
    public partial class CompetencyModel
    {
        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("translations")]
        public ICollection<CompetencyModelTranslation> Translations { get; set; }

        [JsonProperty("behaviors")]
        public ICollection<Behavior> Behaviors { get; set; }

        [JsonProperty("pk")]
        public long? Id { get; set; }
    }

    public partial class CompetencyModelTranslation
    {
        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public partial class Behavior
    {
        [JsonProperty("translations")]
        public ICollection<CompetencyModelTranslation> Translations { get; set; }

        [JsonProperty("pk")]
        public long? Id { get; set; }
    }
}
