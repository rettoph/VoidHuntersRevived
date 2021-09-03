using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using tainicom.Aether.Physics2D.Collision.Shapes;
using VoidHuntersRevived.Library.Contexts.Utilities;
using VoidHuntersRevived.Library.Extensions.System.Text.Json;

namespace VoidHuntersRevived.Library.Json.JsonConverters.Utilities
{
    internal class ConnectionNodeDtoJsonConverter : JsonConverter<ConnectionNodeDto>
    {
        private enum Properties
        {
            Position,
            Rotation
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(ConnectionNodeDto);
        }

        public override ConnectionNodeDto Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ConnectionNodeDto value = new ConnectionNodeDto();

            reader.CheckToken(JsonTokenType.StartObject);
            reader.Read();

            while(reader.ReadProperty(out Properties property))
            {
                switch (property)
                {
                    case Properties.Position:
                        value.Position = JsonSerializer.Deserialize<Vector2>(ref reader, options);
                        reader.Read();
                        break;
                    case Properties.Rotation:
                        value.Rotation = reader.ReadSingle();
                        break;
                }

            }

            reader.CheckToken(JsonTokenType.EndObject);

            return value;
        }

        public override void Write(Utf8JsonWriter writer, ConnectionNodeDto value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(Properties.Position);
            JsonSerializer.Serialize(writer, value.Position, options);

            writer.WriteNumber(Properties.Rotation, value.Rotation);

            writer.WriteEndObject();
        }
    }
}
