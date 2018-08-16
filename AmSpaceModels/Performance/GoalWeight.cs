using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace AmSpaceModels.Performance
{
    public partial class GoalWeight
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("weight")]
        public long Weight { get; set; }
    }
}
