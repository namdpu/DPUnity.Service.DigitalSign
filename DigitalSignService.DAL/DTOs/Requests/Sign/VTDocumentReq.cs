using Newtonsoft.Json;

namespace DigitalSignService.DAL.DTOs.Requests.Sign
{
    public class VTDocumentReq
    {
        public VTDocumentReq(string documentId, string documentName)
        {
            this.DocumentId = documentId;
            this.DocumentName = documentName;
        }

        [JsonProperty("document_id")]
        public string DocumentId { get; set; }

        [JsonProperty("document_name")]
        public string DocumentName { get; set; }
    }
}
