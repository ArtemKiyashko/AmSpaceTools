using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AmSpaceModels.Organization
{
    public partial class SearchUsers
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("previous")]
        public object Previous { get; set; }

        [JsonProperty("results")]
        public List<SearchUserResult> Results { get; set; }
    }

    public partial class SearchUserResult
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("avatar")]
        public object Avatar { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("contracts")]
        public List<ContractSearch> Contracts { get; set; }

        [JsonProperty("roles")]
        public List<string> Roles { get; set; }

        [JsonProperty("person_legal_id")]
        public string PersonLegalId { get; set; }
    }

    public partial class ContractSearch
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("domain")]
        public long Domain { get; set; }

        [JsonProperty("domain_name")]
        public string DomainName { get; set; }

        [JsonProperty("brand_name")]
        public string BrandName { get; set; }

        [JsonProperty("position_name")]
        public string PositionName { get; set; }

        [JsonProperty("contract_number")]
        public int ContractNumber { get; set; }
    }
}
