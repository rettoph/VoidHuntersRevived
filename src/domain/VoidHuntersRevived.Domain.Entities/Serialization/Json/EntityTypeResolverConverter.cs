using Guppy.Core.Resources.Common;
using Svelto.ECS;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Domain.Entities.Serialization.Json
{
    internal sealed class EntityTypeResolverConverter : JsonConverter<ResourceResolver<IEntityType>>
    {
        public override ResourceResolver<IEntityType>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string key = string.Empty;
            EntityTypeFlags flags = EntityTypeFlags.None;
            Resource<IEntityType>? baseType = null;
            VoidHuntersEntityDescriptor? descriptor = null;
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
                    case nameof(IEntityType.Flags):
                        flags = JsonSerializer.Deserialize<EntityTypeFlags>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(IEntityType.Descriptor):
                        descriptor = JsonSerializer.Deserialize<VoidHuntersEntityDescriptor>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(IEntityType.BaseType):
                        string baseKey = JsonSerializer.Deserialize<string>(ref reader, options) ?? throw new NotImplementedException();
                        baseType = Resource<IEntityType>.Get(baseKey);
                        reader.Read();
                        break;
                    case nameof(IEntityType.InstanceComponents):
                        instanceComponents = JsonSerializer.Deserialize<Dictionary<Type, IEntityComponent>>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(IEntityType.Components):
                        staticComponents = JsonSerializer.Deserialize<Dictionary<Type, IEntityComponent>>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("Unexpected property name {0}", propertyName));
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            if (baseType is not null)
            {
                return new ResourceResolver<IEntityType>(() => EntityType.Create(key, flags, baseType.Value.Value, instanceComponents.Values, staticComponents.Values));
            }

            if (descriptor is not null)
            {
                return new ResourceResolver<IEntityType>(() => EntityType.Create(key, flags, descriptor, instanceComponents.Values, staticComponents.Values));
            }

            throw new InvalidOperationException(string.Format("{0}::{1} - Either {2} or {3} must be defined.", nameof(EntityTypeResolverConverter), nameof(Read), nameof(IEntityType.BaseType), nameof(IEntityType.Descriptor)));
        }

        public override void Write(Utf8JsonWriter writer, ResourceResolver<IEntityType> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
