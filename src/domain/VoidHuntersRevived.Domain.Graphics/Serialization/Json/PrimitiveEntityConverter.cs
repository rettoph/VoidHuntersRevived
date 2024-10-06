using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;
using VertexPosition = Microsoft.Xna.Framework.Graphics.VertexPosition;

namespace VoidHuntersRevived.Domain.Graphics.Serialization.Json
{
    public class PrimitiveEntityConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsGenericType == false)
            {
                return false;
            }

            bool result = typeToConvert.GetGenericTypeDefinition() == typeof(PrimitiveEntity<>);
            return result;
        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Key<PrimitiveType>? type = default;
            PrimitiveSequenceGroupEnum sequenceGroup = PrimitiveSequenceGroupEnum.Background;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(PrimitiveEntity<VertexPosition>.Type):
                        type = JsonSerializer.Deserialize<Key<PrimitiveType>>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(PrimitiveEntity<VertexPosition>.SequenceGroup):
                        sequenceGroup = JsonSerializer.Deserialize<PrimitiveSequenceGroupEnum>(ref reader, options);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            if (type.HasValue == false)
            {
                throw new NotImplementedException();
            }

            object? instance = Activator.CreateInstance(typeToConvert, [type.Value, sequenceGroup]);

            return instance ?? throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
