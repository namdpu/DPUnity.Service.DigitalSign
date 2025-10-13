using Newtonsoft.Json;

namespace DigitalSignService.DAL.DTOs.Requests.Sign
{
    public class VTWebhookData
    {
        [JsonProperty("meta_data")]
        public MetaData MetaData { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("action_timestamp")]
        public int ActionTimestamp { get; set; }

        [JsonProperty("signature_timestamp")]
        public int SignatureTimestamp { get; set; }

        [JsonProperty("results")]
        public List<Result> Results { get; set; }
    }

    public class MetaData
    {
        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class Result
    {
        [JsonProperty("document_id")]
        public string DocumentId { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }
    }
}
