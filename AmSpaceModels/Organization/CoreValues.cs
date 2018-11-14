using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AmSpaceModels.Organization
{
    public partial class CoreValues
    {
        [JsonProperty("core_values")]
        public ICollection<CoreValue> CoreValuesList { get; set; }

        [JsonProperty("to_delete_core_values")]
        public ICollection<long> ToDeleteCoreValues { get; set; }
    }

    public partial class CoreValue
    {
        [JsonProperty("pk")]
        public long? Id { get; set; }

        [JsonProperty("translations")]
        public ICollection<CoreValueTranslation> Translations { get; set; }
    }

    public partial class CoreValueTranslation
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }
    }
}
