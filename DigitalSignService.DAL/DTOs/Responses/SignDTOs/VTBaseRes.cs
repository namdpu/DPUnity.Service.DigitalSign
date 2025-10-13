using Newtonsoft.Json;

namespace DigitalSignService.DAL.DTOs.Responses.SignDTOs
{
    public class VTBaseRes
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}
