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
    internal class Vector2JsonConverter : JsonConverter<Vector2>
    {
        private enum Properties
        {
            X,
            Y
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Vector2);
        }

        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Single x = default, y = default;

            reader.CheckToken(JsonTokenType.StartObject);
            reader.Read();

            while(reader.ReadProperty(out Properties property))
            {
                switch (property)
                {
                    case Properties.X:
                        x = reader.ReadSingle();
                        break;
                    case Properties.Y:
                        y = reader.ReadSingle();
                        break;
                }

            }

            reader.CheckToken(JsonTokenType.EndObject);

            return new Vector2(x, y);
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(Properties.X, value.X);
            writer.WriteNumber(Properties.Y, value.Y);

            writer.WriteEndObject();
        }
    }
}
