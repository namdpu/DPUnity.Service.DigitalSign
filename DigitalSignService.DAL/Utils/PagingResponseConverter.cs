using DigitalSignService.DAL.DTOs.Responses;
using Newtonsoft.Json;

namespace DigitalSignService.DAL.Utils
{
    public class PagingResponseConverter<T> : JsonConverter<PagingResponse<T>> where T : class
    {
        public override void WriteJson(JsonWriter writer, PagingResponse<T> value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            foreach (var prop in value.AdditionalProperties)
            {
                writer.WritePropertyName(prop.Key);
                serializer.Serialize(writer, prop.Value);
            }

            writer.WritePropertyName("isTruncated");
            writer.WriteValue(value.IsTruncated);

            writer.WritePropertyName("data");
            serializer.Serialize(writer, value.Data);

            writer.WriteEndObject();
        }

        public override PagingResponse<T> ReadJson(JsonReader reader, Type objectType, PagingResponse<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Deserialization is not implemented.");
        }
    }
}
