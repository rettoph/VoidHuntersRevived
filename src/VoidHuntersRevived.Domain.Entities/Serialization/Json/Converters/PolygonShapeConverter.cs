﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;

namespace VoidHuntersRevived.Domain.Entities.Serialization.Json.Converters
{
    internal class PolygonShapeConverter : JsonConverter<PolygonShape>
    {
        public override PolygonShape? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var vertices = new Vertices();
            var density = 0f;

            while(reader.ReadPropertyName(out string? property))
            {
                switch(property)
                {
                    case nameof(PolygonShape.Density):
                        density = reader.ReadSingle();
                        break;
                    case nameof(PolygonShape.Vertices):
                        vertices = JsonSerializer.Deserialize<Vertices>(ref reader, options);
                        reader.Read();
                        break;
                }
            }

            return new PolygonShape(vertices, density); ;
        }

        public override void Write(Utf8JsonWriter writer, PolygonShape value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(nameof(PolygonShape.Density), value.Density);

            writer.WritePropertyName(nameof(PolygonShape.Vertices));
            JsonSerializer.Serialize<Vertices>(writer, value.Vertices, options);

            writer.WriteEndObject();
        }
    }
}
