using Newtonsoft.Json;

namespace DigitalSignService.DAL.DTOs.Responses.SignDTOs
{
    public class VTLoginRes
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
