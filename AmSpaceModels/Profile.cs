using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels
{
    public partial class Profile
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("is_superadmin")]
        public bool? IsSuperadmin { get; set; }

        [JsonProperty("is_supervisor")]
        public bool? IsSupervisor { get; set; }

        [JsonProperty("is_instructor")]
        public bool? IsInstructor { get; set; }

        [JsonProperty("is_admin")]
        public bool? IsAdmin { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("date_of_birth")]
        public System.DateTimeOffset DateOfBirth { get; set; }

        [JsonProperty("nationality")]
        public string Nationality { get; set; }

        [JsonProperty("sex")]
        public long? Sex { get; set; }

        [JsonProperty("motto")]
        public string Motto { get; set; }

        [JsonProperty("facebook_username")]
        public string FacebookUsername { get; set; }

        [JsonProperty("instagram_username")]
        public string InstagramUsername { get; set; }

        [JsonProperty("linkedin_username")]
        public string LinkedinUsername { get; set; }

        [JsonProperty("pinterest_username")]
        public string PinterestUsername { get; set; }

        [JsonProperty("twitter_username")]
        public string TwitterUsername { get; set; }

        [JsonProperty("tumblr_username")]
        public string TumblrUsername { get; set; }

        [JsonProperty("contract_data")]
        public List<ContractDatum> ContractData { get; set; }

        [JsonProperty("count_certificates")]
        public CountCertificates CountCertificates { get; set; }

        [JsonProperty("competencies")]
        public object[] Competencies { get; set; }

        [JsonProperty("development_activity")]
        public object DevelopmentActivity { get; set; }

        [JsonProperty("educational_information")]
        public EducationalInformation EducationalInformation { get; set; }

        [JsonProperty("jobs_length")]
        public JobsLength JobsLength { get; set; }

        [JsonProperty("language_information")]
        public object[] LanguageInformation { get; set; }

        [JsonProperty("count_recognitions")]
        public long? CountRecognitions { get; set; }

        [JsonProperty("mobility")]
        public Mobility Mobility { get; set; }

        [JsonProperty("user_recognitions")]
        public object[] UserRecognitions { get; set; }

        [JsonProperty("is_subordinate")]
        public bool? IsSubordinate { get; set; }

        [JsonProperty("selected_contract")]
        public SelectedContract[] SelectedContract { get; set; }

        [JsonProperty("is_active")]
        public bool? IsActive { get; set; }

        [JsonProperty("is_country_admin")]
        public bool? IsCountryAdmin { get; set; }

        [JsonProperty("successor_summary")]
        public SuccessorSummary SuccessorSummary { get; set; }

        [JsonProperty("is_email_hidden")]
        public bool? IsEmailHidden { get; set; }

        [JsonProperty("is_phone_number_hidden")]
        public bool? IsPhoneNumberHidden { get; set; }

        [JsonProperty("is_date_of_birth_hidden")]
        public bool? IsDateOfBirthHidden { get; set; }

        [JsonProperty("scopes")]
        public Scopes Scopes { get; set; }

        [JsonProperty("can_develop")]
        public bool? CanDevelop { get; set; }
    }

    public partial class ContractDatum
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("manager")]
        public ContractDatumManager Manager { get; set; }

        [JsonProperty("additional_approver")]
        public object AdditionalApprover { get; set; }

        [JsonProperty("position")]
        public Position Position { get; set; }

        [JsonProperty("level")]
        public ProfileLevel Level { get; set; }

        [JsonProperty("domain")]
        public Domain Domain { get; set; }

        [JsonProperty("brand_name")]
        public object BrandName { get; set; }
    }

    public partial class Domain
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("active")]
        public bool? Active { get; set; }

        [JsonProperty("mpk")]
        public long? Mpk { get; set; }

        [JsonProperty("search")]
        public string Search { get; set; }

        [JsonProperty("parent")]
        public long? Parent { get; set; }
    }

    public partial class ProfileLevel
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("level")]
        public long? LevelLevel { get; set; }
    }

    public partial class ContractDatumManager
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("date_of_birth")]
        public System.DateTimeOffset DateOfBirth { get; set; }

        [JsonProperty("nationality")]
        public string Nationality { get; set; }

        [JsonProperty("sex")]
        public long? Sex { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("employee_id")]
        public long? EmployeeId { get; set; }

        [JsonProperty("status")]
        public long? Status { get; set; }

        [JsonProperty("start_date")]
        public System.DateTimeOffset StartDate { get; set; }

        [JsonProperty("end_date")]
        public object EndDate { get; set; }

        [JsonProperty("dosplus")]
        public string Dosplus { get; set; }

        [JsonProperty("created_at")]
        public System.DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public System.DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("domain")]
        public long? Domain { get; set; }

        [JsonProperty("user")]
        public long? User { get; set; }

        [JsonProperty("level")]
        public long? Level { get; set; }

        [JsonProperty("position")]
        public long? Position { get; set; }

        [JsonProperty("manager")]
        public long? Manager { get; set; }

        [JsonProperty("additional_approver")]
        public object AdditionalApprover { get; set; }

        [JsonProperty("subarea")]
        public object Subarea { get; set; }

        [JsonProperty("type")]
        public object Type { get; set; }
    }

    public partial class Position
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("position_id")]
        public long? PositionId { get; set; }

        [JsonProperty("is_active")]
        public bool? IsActive { get; set; }
    }

    public partial class CountCertificates
    {
        [JsonProperty("internal")]
        public long? Internal { get; set; }

        [JsonProperty("external")]
        public long? External { get; set; }
    }

    public partial class EducationalInformation
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("school_name")]
        public string SchoolName { get; set; }

        [JsonProperty("specialization")]
        public string Specialization { get; set; }

        [JsonProperty("degree")]
        public string Degree { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("education_begin_date")]
        public System.DateTimeOffset EducationBeginDate { get; set; }

        [JsonProperty("education_end_date")]
        public object EducationEndDate { get; set; }
    }

    public partial class JobsLength
    {
        [JsonProperty("internal")]
        public Ternal Internal { get; set; }

        [JsonProperty("external")]
        public Ternal External { get; set; }
    }

    public partial class Ternal
    {
        [JsonProperty("years")]
        public long? Years { get; set; }

        [JsonProperty("months")]
        public long? Months { get; set; }
    }

    public partial class Mobility
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("abroad_relocate")]
        public bool? AbroadRelocate { get; set; }

        [JsonProperty("abroad_how_long")]
        public long? AbroadHowLong { get; set; }

        [JsonProperty("department")]
        public long[] Department { get; set; }

        [JsonProperty("brand")]
        public long[] Brand { get; set; }

        [JsonProperty("abroad_preferred_city")]
        public string AbroadPreferredCity { get; set; }

        [JsonProperty("country_relocate")]
        public bool? CountryRelocate { get; set; }

        [JsonProperty("country_how_long")]
        public long? CountryHowLong { get; set; }

        [JsonProperty("country_preferred_city")]
        public string CountryPreferredCity { get; set; }

        [JsonProperty("change_brand")]
        public bool? ChangeBrand { get; set; }

        [JsonProperty("change_department")]
        public bool? ChangeDepartment { get; set; }
    }

    public partial class Scopes
    {
        [JsonProperty("can_manage_learning")]
        public bool? CanManageLearning { get; set; }

        [JsonProperty("can_manage_performance")]
        public bool? CanManagePerformance { get; set; }

        [JsonProperty("can_manage_helpdesk")]
        public bool? CanManageHelpdesk { get; set; }

        [JsonProperty("can_manage_personal_development")]
        public bool? CanManagePersonalDevelopment { get; set; }

        [JsonProperty("can_manage_surveys")]
        public bool? CanManageSurveys { get; set; }

        [JsonProperty("can_manage_succession")]
        public bool? CanManageSuccession { get; set; }

        [JsonProperty("can_manage_reports")]
        public bool? CanManageReports { get; set; }

        [JsonProperty("can_manage_statistics")]
        public bool? CanManageStatistics { get; set; }

        [JsonProperty("can_manage_notifications")]
        public bool? CanManageNotifications { get; set; }
    }

    public partial class SelectedContract
    {
        [JsonProperty("domain_id")]
        public long DomainId { get; set; }

        [JsonProperty("employee_id")]
        public long EmployeeId { get; set; }

        [JsonProperty("start_date")]
        public System.DateTimeOffset StartDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("manager")]
        public SelectedContractManager Manager { get; set; }
    }

    public partial class SelectedContractManager
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("user_id")]
        public long? UserId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class SuccessorSummary
    {
        [JsonProperty("successors")]
        public long? Successors { get; set; }

        [JsonProperty("chosen_for_succession")]
        public long? ChosenForSuccession { get; set; }
    }
}
