using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Physics.Common;

namespace VoidHuntersRevived.Domain.Physics.Serialization.Json.Converters
{
    public class PolygonConverter : JsonConverter<Polygon>
    {
        public override Polygon Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            FixVector2[] vertices = [];
            Fix64 density = Fix64.Zero;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? property))
            {
                switch (property)
                {
                    case nameof(Polygon.Density):
                        density = JsonSerializer.Deserialize<Fix64>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(Polygon.Vertices):
                        vertices = JsonSerializer.Deserialize<FixVector2[]>(ref reader, options) ?? [];
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new Polygon(density, vertices); ;
        }

        public override void Write(Utf8JsonWriter writer, Polygon value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Polygon.Density), value.Density.RawValue);

            writer.WritePropertyName(nameof(Polygon.Vertices));
            JsonSerializer.Serialize(writer, value.Vertices, options);

            writer.WriteEndObject();
        }
    }
}
