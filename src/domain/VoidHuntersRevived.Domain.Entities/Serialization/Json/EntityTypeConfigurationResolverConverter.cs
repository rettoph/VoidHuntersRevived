using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.Services;
using Guppy.Core.Serialization.Common.Services;
using Svelto.ECS;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Serialization.Json
{
    internal sealed class EntityTypeConfigurationResolverConverter : JsonConverter<ResourceResolver<EntityTypeConfiguration>>
    {
        private readonly Lazy<IResourceService> _resourceService;
        private readonly IPolymorphicJsonSerializerService<IEntityType> _entityTypeTypeService;


        public EntityTypeConfigurationResolverConverter(
            Lazy<IResourceService> resourceService,
            IPolymorphicJsonSerializerService<IEntityType> entityTypeTypeService)
        {
            _resourceService = resourceService;
            _entityTypeTypeService = entityTypeTypeService;
        }

        public override ResourceResolver<EntityTypeConfiguration>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Key<IEntityType>? key = null;
            Type? type = null;
            EntityTypeFlags flags = EntityTypeFlags.None;
            Key<IEntityType>[] include = Array.Empty<Key<IEntityType>>();
            Dictionary<Type, IEntityComponent> components = new Dictionary<Type, IEntityComponent>();
            Dictionary<Type, IEntityComponent> typeEntityComponents = new Dictionary<Type, IEntityComponent>();

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(EntityTypeConfiguration.Key):
                        key = JsonSerializer.Deserialize<Key<IEntityType>>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(EntityTypeConfiguration.Type):
                        type = _entityTypeTypeService.GetType(reader.ReadString());
                        break;
                    case nameof(EntityTypeConfiguration.Flags):
                        flags = JsonSerializer.Deserialize<EntityTypeFlags>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(EntityTypeConfiguration.Include):
                        include = JsonSerializer.Deserialize<Key<IEntityType>[]>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(EntityTypeConfiguration.Components):
                        components = JsonSerializer.Deserialize<Dictionary<Type, IEntityComponent>>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("Unexpected property name {0}", propertyName));
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            if (key is null)
            {
                throw new InvalidDataException();
            }

            return new ResourceResolver<EntityTypeConfiguration>(() =>
            {
                EntityTypeConfiguration entityTypeConfiguration = new EntityTypeConfiguration()
                {
                    Key = key.Value,
                    Type = type,
                    Flags = flags,
                    Components = components,
                    Include = include,
                };

                return entityTypeConfiguration;
            });
        }

        public override void Write(Utf8JsonWriter writer, ResourceResolver<EntityTypeConfiguration> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
