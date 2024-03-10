using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal sealed class PieceTypeConverter : JsonConverter<PieceType>
    {
        public override PieceType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string key = string.Empty;
            VoidHuntersEntityDescriptor descriptor = default!;
            Dictionary<Type, IPieceComponent> instanceComponents = default!;
            Dictionary<Type, IPieceComponent> staticComponents = default!;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(PieceType.Key):
                        key = JsonSerializer.Deserialize<string>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(PieceType.Descriptor):
                        descriptor = JsonSerializer.Deserialize<VoidHuntersEntityDescriptor>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(PieceType.InstanceComponents):
                        instanceComponents = JsonSerializer.Deserialize<Dictionary<Type, IPieceComponent>>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(PieceType.StaticComponents):
                        staticComponents = JsonSerializer.Deserialize<Dictionary<Type, IPieceComponent>>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new PieceType(key, descriptor, instanceComponents ?? new Dictionary<Type, IPieceComponent>(), staticComponents ?? new Dictionary<Type, IPieceComponent>());
        }

        public override void Write(Utf8JsonWriter writer, PieceType value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
