using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal class PlugJsonConverter : JsonConverter<Plug>
    {
        public override Plug Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Location location = default;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(Plug.Location):
                        location = JsonSerializer.Deserialize<Location>(ref reader, options);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new Plug()
            {
                Location = location
            };
        }

        public override void Write(Utf8JsonWriter writer, Plug value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
