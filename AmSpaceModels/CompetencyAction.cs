using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels
{
    public partial class CompetencyAction
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("plan_type")]
        public ActionType PlanType { get; set; }

        [JsonProperty("level")]
        public long LevelId { get; set; }

        [JsonProperty("goals")]
        public List<object> Goals { get; set; }

        [JsonProperty("actions")]
        public List<IdpAction> Actions { get; set; }

        [JsonProperty("theory_actions_count")]
        public long TheoryActionsCount { get; set; }

        [JsonProperty("feedback_actions_count")]
        public long FeedbackActionsCount { get; set; }

        [JsonProperty("practice_actions_count")]
        public long PracticeActionsCount { get; set; }

        [JsonProperty("translations")]
        public List<Translation> Translations { get; set; }
    }
}
