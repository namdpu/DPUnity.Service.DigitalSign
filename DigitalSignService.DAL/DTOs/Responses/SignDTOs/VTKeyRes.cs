using Newtonsoft.Json;

namespace CloudCANetCore.BO
{
    public class VTKeyRes
    {

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("algo")]
        public string[] Algo { get; set; }

        [JsonProperty("len")]
        public int Len { get; set; }
    }
}
