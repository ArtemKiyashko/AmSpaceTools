using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AmSpaceModels.Performance
{
    public partial class Goal
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("weight")]
        public int? Weight { get; set; }

        [JsonProperty("perspective")]
        public Perspective Perspective { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("is_deleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("created_by")]
        public long? CreatedBy { get; set; }

        [JsonProperty("updated_by")]
        public long? UpdatedBy { get; set; }

        [JsonProperty("kpi")]
        public Kpi Kpi { get; set; }

        [JsonProperty("kpi_category")]
        public KpiCategory KpiCategory { get; set; }

        [JsonProperty("roadmap")]
        public long Roadmap { get; set; }

        [JsonProperty("added_from")]
        public AddedFrom AddedFrom { get; set; }

        [JsonProperty("related_added_by")]
        public List<object> RelatedAddedBy { get; set; }

        [JsonProperty("priorities")]
        public List<Priority> Priorities { get; set; }

        [JsonProperty("priorities_count")]
        public long PrioritiesCount { get; set; }

        [JsonProperty("realised_count")]
        public long RealisedCount { get; set; }
    }

    public partial class AddedFrom
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
    }

    public partial class Priority
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("kpi_text")]
        public string KpiText { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("is_deleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("kpi")]
        public object Kpi { get; set; }

        [JsonProperty("kpi_category")]
        public object KpiCategory { get; set; }

        [JsonProperty("determinant_type")]
        public string DeterminantType { get; set; }

        [JsonProperty("determinant_id")]
        public long DeterminantId { get; set; }

        [JsonProperty("determinant")]
        public Determinant Determinant { get; set; }

        [JsonProperty("month")]
        public long Month { get; set; }
    }

    public partial class Determinant
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("perspective")]
        public long Perspective { get; set; }
    }
}
