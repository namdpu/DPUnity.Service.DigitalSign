using Newtonsoft.Json;

namespace DigitalSignService.DAL.DTOs.Requests.Sign
{
    public class VTLoginReq
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("profile_id")]
        public string ProfileId { get; set; }
    }
}
