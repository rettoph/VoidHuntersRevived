using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Serialization.Json
{
    internal class FixTransform2DConverter : JsonConverter<FixTransform2D>
    {
        public override FixTransform2D Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Fix64 x = default, y = default, rotation = default;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(FixTransform2D.X):
                        x = JsonSerializer.Deserialize<Fix64>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(FixTransform2D.Y):
                        y = JsonSerializer.Deserialize<Fix64>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(FixTransform2D.Rotation):
                        rotation = JsonSerializer.Deserialize<Fix64>(ref reader, options);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new FixTransform2D(x, y, rotation);
        }

        public override void Write(Utf8JsonWriter writer, FixTransform2D value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
