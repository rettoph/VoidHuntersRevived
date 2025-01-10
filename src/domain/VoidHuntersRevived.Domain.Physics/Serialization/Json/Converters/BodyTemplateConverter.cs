using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Physics.Common;

namespace VoidHuntersRevived.Domain.Physics.Serialization.Json
{
    public class BodyTemplateConverter : JsonConverter<IBodyTemplate>
    {
        public override IBodyTemplate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            FixVector2 centeroid = default;
            Polygon[] shapes = [];

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(IBodyTemplate.Centeroid):
                        centeroid = JsonSerializer.Deserialize<FixVector2>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(IBodyTemplate.Shapes):
                        shapes = JsonSerializer.Deserialize<Polygon[]>(ref reader, options) ?? [];
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new BodyTemplate()
            {
                Centeroid = centeroid,
                Shapes = shapes
            };
        }

        public override void Write(Utf8JsonWriter writer, IBodyTemplate value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}