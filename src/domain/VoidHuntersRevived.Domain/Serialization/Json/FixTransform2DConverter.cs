using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Serialization.Json
{
    internal class FixTransform2DConverter : JsonConverter<FixTransform2D>
    {
        public override FixTransform2D Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            FixVector2 position = default;
            FixComplex rotation = default;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(FixTransform2D.Position):
                        position = JsonSerializer.Deserialize<FixVector2>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(FixTransform2D.Radians):
                        Fix64 radians = JsonSerializer.Deserialize<Fix64>(ref reader, options);
                        rotation = new FixComplex(radians);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new FixTransform2D(rotation, position);
        }

        public override void Write(Utf8JsonWriter writer, FixTransform2D value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
