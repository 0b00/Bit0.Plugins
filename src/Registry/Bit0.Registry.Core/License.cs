using System;
using Newtonsoft.Json;

namespace Bit0.Registry.Core
{
    public class License
    {
        [JsonProperty("name")]
        public String Name { get; set; }
        
        [JsonProperty("url")]
        public String Url { get; set; }
    }
}