using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AmSpaceModels.Organization
{
    public partial class TemporaryAccount
    {
        [JsonProperty("domain")]
        public long DomainId { get; set; }

        [JsonProperty("position")]
        public long PositionId { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("person_legal_id")]
        public string PersonLegalId { get; set; }

        [JsonProperty("re_person_legal_id")]
        public string RePersonLegalId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
}
