using Svelto.DataStructures;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Physics;

namespace VoidHuntersRevived.Domain.Physics.Serialization.Json.Converters
{
    internal class PolygonConverter : JsonConverter<Polygon>
    {
        public override Polygon Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            NativeDynamicArrayCast<FixVector2> vertices = default;
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
                        vertices = JsonSerializer.Deserialize<NativeDynamicArrayCast<FixVector2>>(ref reader, options);
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
            JsonSerializer.Serialize<NativeDynamicArray>(writer, value.Vertices.ToNativeArray(), options);

            writer.WriteEndObject();
        }
    }
}
