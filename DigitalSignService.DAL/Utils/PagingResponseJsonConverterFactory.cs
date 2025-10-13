using DigitalSignService.DAL.DTOs.Responses;
using Newtonsoft.Json;

namespace DigitalSignService.DAL.Utils
{
    public class PagingResponseJsonConverterFactory : JsonConverter
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsGenericType &&
                   typeToConvert.GetGenericTypeDefinition() == typeof(PagingResponse<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Deserialization is not implemented.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var genericArgument = value.GetType().GetGenericArguments()[0];
            var converterType = typeof(PagingResponseConverter<>).MakeGenericType(genericArgument);

            var converterInstance = (JsonConverter)Activator.CreateInstance(converterType);

            converterInstance.WriteJson(writer, value, serializer);
        }
    }
}
