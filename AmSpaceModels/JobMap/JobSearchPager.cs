using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.JobMap
{
    public class JobSearchPager
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("previous")]
        public string Previous { get; set; }

        [JsonProperty("results")]
        public List<JobMap> Results { get; set; }
    }

    public class JobMap
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("translations")]
        public List<JobDescriptionTranslation> JobDescriptionsTranslations { get; set; }

        [JsonProperty("employee_comments")]
        public List<EmployeeComment> EmployeeComments { get; set; }

        [JsonProperty("responsibilities")]
        public List<JobResponsibility> JobResponsibilities { get; set; }
    }

    public partial class EmployeeComment
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("job_id")]
        public long JobId { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("user_avatar")]
        public Uri UserAvatar { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }
    }

}
