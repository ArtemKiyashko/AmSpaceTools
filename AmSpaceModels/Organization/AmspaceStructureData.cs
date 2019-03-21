using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmSpaceModels.Organization
{
    public class AmspaceDomain : ICopyable<AmspaceDomain>
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mpk")]
        public long? Mpk { get; set; }

        [JsonProperty("children")]
        public IEnumerable<AmspaceDomain> Children { get; set; }

        public AmspaceDomain ShallowCopy()
        {
            return new AmspaceDomain
            {
                Id = this.Id,
                Name = this.Name,
                Mpk = this.Mpk,
                Children = this.Children
            };
        }
    }

    public class AmspaceUser
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

}
