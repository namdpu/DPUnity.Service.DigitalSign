using Newtonsoft.Json;

namespace DigitalSignService.DAL.DTOs.Requests.Sign
{
    public class VTCredentialsReq
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("profile_id")]
        public string ProfileId { get; set; }

        [JsonProperty("certificates")]
        public string Certificates { get; set; }

        [JsonProperty("certInfo")]
        public Boolean CertInfo { get; set; }

        [JsonProperty("authInfo")]
        public Boolean AuthInfo { get; set; }
    }
}
