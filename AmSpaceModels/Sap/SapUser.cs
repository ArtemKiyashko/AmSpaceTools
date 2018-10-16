using AmSpaceModels.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.Sap
{
    public class SapUserDelete
    {
        [JsonProperty("employee_id")]
        public int EmployeeId { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        public SapUserDelete(int employeeId)
        {
            EmployeeId = employeeId;
            Date = DateTime.Today.ToShortDateString();
            Status = 0;
        }
    }

    public class SapUser
    {

        [JsonProperty("ldap_name")]
        public string LdapName { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("person_legal_id")]
        public string PersonLegalId { get; set; }

        [JsonProperty("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty("nationality")]
        public string Nationality { get; set; }

        [JsonProperty("sex")]
        public Sex Sex { get; set; }

        [JsonProperty("main_employee_id")]
        public int MainEmployeeId { get; set; }

        [JsonProperty("employee_id")]
        public int EmployeeId { get; set; }

        [JsonProperty("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("end_date")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("domain_id")]
        public int DomainId { get; set; }

        [JsonProperty("position_id")]
        public int? PositionId { get; set; }

        [JsonProperty("position_name")]
        public string PositionName { get; set; }

        [JsonProperty("manager_employee_id")]
        public int? ManagerEmployeeId { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("dosplus")]
        public string Dosplus { get; set; }
    }
}
