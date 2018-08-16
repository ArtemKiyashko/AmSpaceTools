using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AmSpaceModels.Performance
{
    public partial class Roadmaps
    {
        [JsonProperty("managers")]
        public Managers Managers { get; set; }

        [JsonProperty("results")]
        public List<Roadmap> Results { get; set; }
    }

    public partial class Managers
    {
        [JsonProperty("manager")]
        public Manager Manager { get; set; }

        [JsonProperty("second_level_manager")]
        public Manager SecondLevelManager { get; set; }
    }

    public partial class Manager
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("contract_pk")]
        public long ContractPk { get; set; }
    }

    public partial class Roadmap
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("year")]
        public long? Year { get; set; }

        [JsonProperty("status")]
        public RoadmapStatus? Status { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty("contract")]
        public Contract Contract { get; set; }

        [JsonProperty("is_available")]
        public bool? IsAvailable { get; set; }
    }

    public enum RoadmapStatus
    {
        ROADMAP_IN_PROGRESS,
        ROADMAP_ACCEPTED,
        ROADMAP_PENDING,
        ROADMAP_FILLED
    }
}
