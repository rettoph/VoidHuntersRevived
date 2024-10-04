using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Graphics.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Contexts;

namespace VoidHuntersRevived.Domain.Graphics.Serialization.Json
{
    public class PrimitiveContextConverter : JsonConverter<PrimitiveContext>
    {
        public override PrimitiveContext Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Key<IPrimitive>? type = default;
            int sequence = 0;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(PrimitiveContext.Type):
                        type = JsonSerializer.Deserialize<Key<IPrimitive>>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(PrimitiveContext.Sequence):
                        sequence = reader.ReadInt32();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            if (type.HasValue == false)
            {
                throw new NotImplementedException();
            }

            return new PrimitiveContext(type.Value, sequence);
        }

        public override void Write(Utf8JsonWriter writer, PrimitiveContext value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
