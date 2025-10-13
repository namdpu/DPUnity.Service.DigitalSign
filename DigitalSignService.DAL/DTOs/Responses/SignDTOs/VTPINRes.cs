using Newtonsoft.Json;

namespace CloudCANetCore.BO
{
    public class VTPINRes
    {

        [JsonProperty("presence")]
        public string presence { get; set; }

        [JsonProperty("format")]
        public string format { get; set; }

        [JsonProperty("label")]
        public string label { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }
    }
}
