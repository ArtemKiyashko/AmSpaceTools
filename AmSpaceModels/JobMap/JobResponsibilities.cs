﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.JobMap
{
    public class JobResponsibilities
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("job")]
        public long Job { get; set; }

        [JsonProperty("translations")]
        public List<ResponsibilityTranslation> Translations { get; set; }
    }

    public class ResponsibilityTranslation
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("kpi_text")]
        public string KpiText { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }
    }
}
