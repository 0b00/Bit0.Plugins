using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bit0.Registry.Core
{
    public class Package
    {
        [JsonExtensionData]
        private readonly IDictionary<String, JToken> _additionalData = new Dictionary<String, JToken>();

        [JsonProperty("id")]
        public String Id { get; set; }
        [JsonProperty("name")]
        public String Name { get; set; }
        [JsonProperty("description")]
        public String Description { get; set; }
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PackageType Type { get; set; }
        [JsonProperty("published")]
        public DateTime Published { get; set; }
        [JsonProperty("updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("license")]
        public License License { get; set; }
        [JsonProperty("author")]
        public Author Author { get; set; }
        [JsonProperty("homepage")]
        public String Homepage { get; set; }
        [JsonProperty("icon")]
        public String Icon { get; set; }
        [JsonProperty("screenshot")]
        public String Screenshot { get; set; }
        [JsonProperty("tags")]
        public IEnumerable<String> Tags { get; set; }
        [JsonProperty("features")]
        public IEnumerable<String> Features { get; set; }
        [JsonIgnore]
        public IEnumerable<Package> Dependencies { get; set; }
        [JsonProperty("versions")]
        public IEnumerable<PackageVersion> Versions { get; set; }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            // TODO: Dependencies

            // var depsJson = _additionalData["deps"] as IDictionary<String, String>;
        }
    }
}
