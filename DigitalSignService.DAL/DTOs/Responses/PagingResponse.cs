using DigitalSignService.DAL.Utils;
using System.Text.Json.Serialization;

namespace DigitalSignService.DAL.DTOs.Responses
{
    [JsonConverter(typeof(PagingResponseConverter<>))]
    //[JsonConverter(typeof(PagingResponseJsonConverterFactory))]
    public class PagingResponse<T> where T : class
    {
        public bool IsTruncated { get; set; }
        public IEnumerable<T> Data { get; set; }
        [JsonIgnore] // Prevent direct serialization of AdditionalProperties
        public Dictionary<string, object> AdditionalProperties { get; set; } = new Dictionary<string, object>();

        public void AddProperty(string key, object value)
        {
            AdditionalProperties[key] = value;
        }
    }
}
