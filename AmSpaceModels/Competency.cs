using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels
{
    public partial class Competency
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("previous")]
        public object Previous { get; set; }

        [JsonProperty("results")]
        public List<Result> Results { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("plan_type")]
        public long PlanType { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        [JsonProperty("level_id")]
        public long LevelId { get; set; }

        [JsonProperty("action_count")]
        public long ActionCount { get; set; }

        [JsonProperty("goal_count")]
        public long GoalCount { get; set; }

        [JsonProperty("translations")]
        public List<Translation> Translations { get; set; }
    }
}
