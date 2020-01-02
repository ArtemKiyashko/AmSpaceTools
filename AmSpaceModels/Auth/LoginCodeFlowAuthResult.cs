using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceModels.Auth
{
    public class LoginCodeFlowAuthResult
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("session_state")]
        public string SessionState { get; set; }
    }
}
