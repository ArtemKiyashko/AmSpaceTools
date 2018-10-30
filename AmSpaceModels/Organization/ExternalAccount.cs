using System;
using System.Collections.Generic;

using System.Globalization;
using AmSpaceModels.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AmSpaceModels.Organization
{
    public partial class ExternalAccount
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("person_legal_id")]
        public string PersonLegalId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("date_of_birth")]
        public DateTimeOffset? DateOfBirth { get; set; }

        [JsonProperty("nationality")]
        public string Nationality { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("sex")]
        public Sex Sex { get; set; }

        [JsonProperty("mpk")]
        public long Mpk { get; set; }

        [JsonProperty("manager_user_id")]
        public long ManagerId { get; set; }

        [JsonProperty("contract_number")]
        public long ContractNumber { get; set; }

        [JsonProperty("position_name")]
        public string PositionName { get; set; }

        [JsonProperty("position_id")]
        public long? PositionId { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("start_date")]
        public DateTimeOffset StartDate { get; set; }

        [JsonProperty("end_date")]
        public DateTimeOffset? EndDate { get; set; }

        [JsonProperty("status")]
        public UserStatus Status { get; set; }
    }
}
