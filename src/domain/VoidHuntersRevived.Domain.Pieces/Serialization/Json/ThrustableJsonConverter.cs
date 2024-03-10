using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Pieces.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal class ThrustableJsonConverter : JsonConverter<Thrustable>
    {
        public override Thrustable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            FixPolar maxImpulse = default;
            FixVector2 impulsePoint = default;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(Thrustable.MaxImpulse):
                        maxImpulse = JsonSerializer.Deserialize<FixPolar>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(Thrustable.ImpulsePoint):
                        impulsePoint = JsonSerializer.Deserialize<FixVector2>(ref reader, options);
                        reader.Read();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new Thrustable()
            {
                MaxImpulse = maxImpulse,
                ImpulsePoint = impulsePoint
            };
        }

        public override void Write(Utf8JsonWriter writer, Thrustable value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
