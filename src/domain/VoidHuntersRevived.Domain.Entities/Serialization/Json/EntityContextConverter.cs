using Svelto.ECS;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities.Serialization.Json
{
    internal sealed class EntityContextConverter : JsonConverter<EntityContext>
    {
        public override EntityContext? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string key = string.Empty;
            VoidHuntersEntityDescriptor descriptor = default!;
            Dictionary<Type, IEntityComponent> instanceComponents = default!;
            Dictionary<Type, IEntityComponent> staticComponents = default!;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(EntityContext.Key):
                        key = JsonSerializer.Deserialize<string>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(EntityContext.Descriptor):
                        descriptor = JsonSerializer.Deserialize<VoidHuntersEntityDescriptor>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(EntityContext.InstanceComponents):
                        instanceComponents = JsonSerializer.Deserialize<Dictionary<Type, IEntityComponent>>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(EntityContext.StaticComponents):
                        staticComponents = JsonSerializer.Deserialize<Dictionary<Type, IEntityComponent>>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new EntityContext(key, descriptor, instanceComponents ?? new Dictionary<Type, IEntityComponent>(), staticComponents ?? new Dictionary<Type, IEntityComponent>());
        }

        public override void Write(Utf8JsonWriter writer, EntityContext value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
