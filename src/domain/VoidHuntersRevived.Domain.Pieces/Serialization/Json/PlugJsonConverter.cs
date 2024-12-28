using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal class PlugJsonConverter : JsonConverter<Plug>
    {
        public override Plug Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            FixTransform2D nodeTransform = default;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(Plug.NodeTransform):
                        nodeTransform = JsonSerializer.Deserialize<FixTransform2D>(ref reader, options);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new Plug()
            {
                NodeTransform = nodeTransform
            };
        }

        public override void Write(Utf8JsonWriter writer, Plug value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
