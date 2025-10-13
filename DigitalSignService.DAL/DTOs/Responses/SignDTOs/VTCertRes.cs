using Newtonsoft.Json;

namespace CloudCANetCore.BO
{
    public class VTCertRes
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("certificates")]
        public string[] Certificates { get; set; }

        [JsonProperty("issuerDN")]
        public string IssuerDN { get; set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("subjectDN")]
        public string SubjectDN { get; set; }

        [JsonProperty("validFrom")]
        public string ValidFrom { get; set; }

        [JsonProperty("validTo")]
        public string ValidTo { get; set; }
    }
}
