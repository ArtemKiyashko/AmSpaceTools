using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.Idp
{
    public partial class UpdateAction
    {
        [JsonProperty("theory_actions")]
        public List<IdpAction> TheoryActions { get; set; }

        [JsonProperty("feedback_actions")]
        public List<IdpAction> FeedbackActions { get; set; }

        [JsonProperty("practice_actions")]
        public List<IdpAction> PracticeActions { get; set; }
    }
}
