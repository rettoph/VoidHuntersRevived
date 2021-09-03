using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Library.Extensions.System.Text.Json;

namespace VoidHuntersRevived.Library.Json.JsonConverters.Utilities
{
    internal class ShapeJsonConverter : JsonConverter<Shape>
    {
        #region Private Enums
        private enum Properties
        {
            Density,
            ShapeType,
            Vertices
        }
        #endregion

        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(Shape).IsAssignableFrom(typeToConvert);
        }

        public override Shape Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.CheckToken(JsonTokenType.StartObject);
            reader.Read();

            reader.CheckProperty(Properties.ShapeType);
            reader.Read();
            ShapeType shapeType = (ShapeType)Enum.Parse(typeof(ShapeType), reader.ReadString());

            Shape shape = default;
            switch(shapeType)
            {
                case ShapeType.Polygon:
                    shape = this.InnerReadPolygon(ref reader, options);
                    break;
            }

            reader.CheckToken(JsonTokenType.EndObject);

            return shape;
        }

        public override void Write(Utf8JsonWriter writer, Shape value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(Properties.ShapeType, value.ShapeType.ToString());

            switch (value.ShapeType)
            {
                case ShapeType.Polygon:
                    this.InnerWritePolygon(writer, value as PolygonShape, options);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid ShapeType: {value.ShapeType}.");
            }

            writer.WriteEndObject();
        }

        #region Polygon Methods
        private void InnerWritePolygon(Utf8JsonWriter writer, PolygonShape polygon, JsonSerializerOptions options)
        {
            writer.WriteNumber(Properties.Density, polygon.Density);

            writer.WritePropertyName(Properties.Vertices);
            JsonSerializer.Serialize(writer, polygon.Vertices.ToArray(), options);
        }

        private PolygonShape InnerReadPolygon(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Single density = default;
            Vector2[] vertices = default;

            while (reader.ReadProperty(out Properties property))
            {
                switch (property)
                {
                    case Properties.Density:
                        density = reader.ReadSingle();
                        break;
                    case Properties.Vertices:
                        vertices = JsonSerializer.Deserialize<Vector2[]>(ref reader, options);
                        reader.Read();
                        break;
                    default:
                        throw new JsonException();
                }
            }

            return new PolygonShape(new Vertices(vertices), density);
        }
        #endregion
    }
}
