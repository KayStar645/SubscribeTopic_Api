using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Application.Services
{
    public class MessagesConverter : JsonConverter<List<string>>
    {
        public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var message in value)
            {
                writer.WriteStringValue(message);
            }
            writer.WriteEndArray();
        }
    }

}
