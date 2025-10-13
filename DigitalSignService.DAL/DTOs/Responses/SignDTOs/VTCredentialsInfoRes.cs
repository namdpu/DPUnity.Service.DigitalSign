using CloudCANetCore.BO;
using Newtonsoft.Json;

namespace DigitalSignService.DAL.DTOs.Responses.SignDTOs
{
    public class VTCredentialsInfoRes : VTBaseRes
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("key")]
        public VTKeyRes Key { get; set; }

        [JsonProperty("cert")]
        public VTCertRes Cert { get; set; }

        [JsonProperty("PIN")]
        public VTPINRes PIN { get; set; }

        [JsonProperty("OTP")]
        public VTOTPRes OTP { get; set; }

        [JsonProperty("authMode")]
        public string AuthMode { get; set; }

        [JsonProperty("SCAL")]
        public string SCAL { get; set; }

        [JsonProperty("multisign")]
        public int Multisign { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("credential_id")]
        public string CredentialId { get; set; }
    }
}
