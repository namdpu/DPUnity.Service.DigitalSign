using Newtonsoft.Json;

namespace DigitalSignService.DAL.DTOs.Requests.Sign
{
    public class VTSignHashReq
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("credentialID")]
        public string CredentialId { get; set; }

        [JsonProperty("numSignatures")]
        public int NumSignatures { get; set; }

        [JsonProperty("documents")]
        public VTDocumentReq[] Documents { get; set; }

        [JsonProperty("hash")]
        public string[] Hash { get; set; }

        [JsonProperty("hashAlgo")]
        public string HashAlgo { get; set; }

        [JsonProperty("signAlgo")]
        public string SignAlgo { get; set; }

        [JsonProperty("async")]
        public int Async { get; set; }
    }
}
