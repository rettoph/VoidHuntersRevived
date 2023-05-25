using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Physics;

namespace VoidHuntersRevived.Domain.Physics.Serialization.Json.Converters
{
    internal class PolygonConverter : JsonConverter<Polygon>
    {
        public override Polygon? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Vertices vertices = new Vertices();
            Fix64 density = Fix64.Zero;

            while(reader.ReadPropertyName(out string? property))
            {
                switch(property)
                {
                    case nameof(Polygon.Density):
                        density = Fix64.FromRaw(reader.ReadInt64());
                        break;
                    case nameof(Polygon.Vertices):
                        vertices = JsonSerializer.Deserialize<Vertices>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                }
            }

            return new Polygon(vertices, density); ;
        }

        public override void Write(Utf8JsonWriter writer, Polygon value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(Polygon.Density), value.Density.RawValue);

            writer.WritePropertyName(nameof(Polygon.Vertices));
            JsonSerializer.Serialize<Vertices>(writer, value.Vertices, options);

            writer.WriteEndObject();
        }
    }
}
