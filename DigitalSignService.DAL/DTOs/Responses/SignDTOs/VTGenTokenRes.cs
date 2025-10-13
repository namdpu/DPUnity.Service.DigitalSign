using Newtonsoft.Json;

namespace DigitalSignService.DAL.DTOs.Responses.SignDTOs
{
    public class VTGenTokenRes
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("consented_on")]
        public long ConsentedOn { get; set; }
    }
}
