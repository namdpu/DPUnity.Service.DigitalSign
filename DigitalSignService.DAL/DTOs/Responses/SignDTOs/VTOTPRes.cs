using Newtonsoft.Json;

namespace CloudCANetCore.BO
{
    public class VTOTPRes
    {
        [JsonProperty("presence")]
        public string Presence { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("format")]
        public string format { get; set; }

        [JsonProperty("label")]
        public string label { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }
    }
}
