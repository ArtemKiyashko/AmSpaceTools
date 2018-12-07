using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AmSpaceModels.Auth
{
    public partial class NewPassword
    {
        [JsonProperty("new_password")]
        public string Password { get; set; }

        [JsonProperty("re_password")]
        public string RePassword { get; set; }

        [JsonProperty("userId")]
        public long? UserId { get; set; }
    }
}
