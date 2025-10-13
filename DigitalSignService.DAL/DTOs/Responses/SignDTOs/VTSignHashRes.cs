using Newtonsoft.Json;

namespace DigitalSignService.DAL.DTOs.Responses.SignDTOs
{
    public class VTSignHashAsyncRes : VTBaseRes
    {
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }
    }

    public class VTSignHashRes : VTBaseRes
    {
        [JsonProperty("signatures")]
        public string[]? Signatures { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }
}
