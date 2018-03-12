using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels
{
    public partial class UpdateAction
    {
        [JsonProperty("theory_actions")]
        public List<Action> TheoryActions { get; set; }

        [JsonProperty("feedback_actions")]
        public List<Action> FeedbackActions { get; set; }

        [JsonProperty("practice_actions")]
        public List<Action> PracticeActions { get; set; }
    }
}
