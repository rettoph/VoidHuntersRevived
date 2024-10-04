using Microsoft.Xna.Framework.Graphics;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Serialization.Json
{
    public class PrimitiveSequenceGroupConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsGenericType == false)
            {
                return false;
            }

            bool result = typeToConvert.GetGenericTypeDefinition() == typeof(PrimitiveSequenceGroup<>);
            return result;
        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            PrimitiveSequenceGroupEnum value = PrimitiveSequenceGroupEnum.None;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(PrimitiveSequenceGroup<VertexPosition>.Value):
                        value = JsonSerializer.Deserialize<PrimitiveSequenceGroupEnum>(ref reader, options);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            object? instance = Activator.CreateInstance(typeToConvert, [value]);

            return instance ?? throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
