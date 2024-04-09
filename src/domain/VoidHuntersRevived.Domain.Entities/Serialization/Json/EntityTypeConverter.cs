using Svelto.ECS;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Serialization.Json
{
    internal sealed class EntityTypeConverter : JsonConverter<IEntityType>
    {
        public override IEntityType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string key = string.Empty;
            VoidHuntersEntityDescriptor descriptor = default!;
            Dictionary<Type, IEntityComponent> instanceComponents = new Dictionary<Type, IEntityComponent>();
            Dictionary<Type, IEntityComponent> staticComponents = new Dictionary<Type, IEntityComponent>();

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(IEntityType.Key):
                        key = JsonSerializer.Deserialize<string>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(IEntityType.Descriptor):
                        descriptor = JsonSerializer.Deserialize<VoidHuntersEntityDescriptor>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(IEntityType.InstanceComponents):
                        instanceComponents = JsonSerializer.Deserialize<Dictionary<Type, IEntityComponent>>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(IEntityType.StaticComponents):
                        staticComponents = JsonSerializer.Deserialize<Dictionary<Type, IEntityComponent>>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return EntityType.Create(key, descriptor, instanceComponents.Values, staticComponents.Values);
        }

        public override void Write(Utf8JsonWriter writer, IEntityType value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
