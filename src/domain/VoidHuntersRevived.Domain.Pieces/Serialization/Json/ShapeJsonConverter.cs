using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Pieces.Common;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal class ShapeJsonConverter : JsonConverter<Shape>
    {
        public override Shape Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Vector2[] vertices = [];

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(Shape.Vertices):
                        vertices = JsonSerializer.Deserialize<Vector2[]>(ref reader, options) ?? [];
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new Shape()
            {
                Vertices = vertices
            };
        }

        public override void Write(Utf8JsonWriter writer, Shape value, JsonSerializerOptions options) => throw new NotImplementedException();
    }
}