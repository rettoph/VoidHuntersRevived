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
    internal sealed class EntityTypeResolverConverter : JsonConverter<ResourceResolver<IEntityType>>
    {
        private readonly Lazy<IResourceService> _resourceService;
        private readonly IPolymorphicJsonSerializerService<IEntityType> _entityTypeTypeService;


        public EntityTypeResolverConverter(
            Lazy<IResourceService> resourceService,
            IPolymorphicJsonSerializerService<IEntityType> entityTypeTypeService)
        {
            _resourceService = resourceService;
            _entityTypeTypeService = entityTypeTypeService;
        }

        public override ResourceResolver<IEntityType>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            IKey<IEntityType>? key = null;
            EntityTypeFlags flags = EntityTypeFlags.None;
            IKey<IEntityType>[] include = Array.Empty<IKey<IEntityType>>();
            Dictionary<Type, IEntityComponent> components = new Dictionary<Type, IEntityComponent>();
            Dictionary<Type, IEntityComponent> typeEntityComponents = new Dictionary<Type, IEntityComponent>();

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(IEntityType.Key):
                        key = JsonSerializer.Deserialize<IKey<IEntityType>>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(IEntityType.Flags):
                        flags = JsonSerializer.Deserialize<EntityTypeFlags>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(IEntityType.Include):
                        string[] includeNames = JsonSerializer.Deserialize<string[]>(ref reader, options) ?? throw new NotImplementedException();
                        include = includeNames.Select(x => Key.GetByName<IEntityType>(x)).ToArray();
                        reader.Read();
                        break;
                    case nameof(IEntityType.Components):
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

            return new ResourceResolver<IEntityType>(() =>
            {
                EntityType entityType = (EntityType)(Activator.CreateInstance(key.Type, [key, include]) ?? throw new NotImplementedException());

                entityType.WithFlags(flags)
                    .WithComponents(components.Values);

                return entityType;
            });
        }

        public override void Write(Utf8JsonWriter writer, ResourceResolver<IEntityType> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
