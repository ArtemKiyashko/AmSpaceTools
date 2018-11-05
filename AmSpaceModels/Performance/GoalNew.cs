using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AmSpaceModels.Performance
{
    public partial class GoalNew
    {
        [JsonProperty("kpi")]
        public long Kpi { get; set; }

        [JsonProperty("perspective")]
        public Perspective Perspective { get; set; }

        [JsonProperty("kpi_category")]
        public KpiCategory KpiCategory { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public enum Perspective
    {
        PEOPLE,
        CUSTOMER,
        PROFITABILITY,
        GROWTH
    }
}
