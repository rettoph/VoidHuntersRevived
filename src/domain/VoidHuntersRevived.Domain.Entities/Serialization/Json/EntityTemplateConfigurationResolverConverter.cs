using Guppy.Core.Resources.Common;
using Guppy.Core.Serialization.Common.Services;
using Svelto.ECS;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Serialization.Json
{
    internal sealed class EntityTemplateConfigurationResolverConverter(
        IPolymorphicJsonSerializerService<IEntityTemplate> entityTemplateTypeService) : JsonConverter<ResourceResolver<EntityTemplateConfiguration>>
    {
        private readonly IPolymorphicJsonSerializerService<IEntityTemplate> _entityTemplateTypeService = entityTemplateTypeService;

        public override ResourceResolver<EntityTemplateConfiguration>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Key<IEntityTemplate>? key = null;
            Type? type = null;
            EntityTemplateFlags flags = EntityTemplateFlags.None;
            Key<IEntityTemplate>[] include = Array.Empty<Key<IEntityTemplate>>();
            Dictionary<Type, IEntityComponent> components = [];
            Dictionary<Type, IEntityComponent> typeEntityComponents = [];

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(EntityTemplateConfiguration.Key):
                        key = JsonSerializer.Deserialize<Key<IEntityTemplate>>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(EntityTemplateConfiguration.Type):
                        type = _entityTemplateTypeService.GetType(reader.ReadString());
                        break;
                    case nameof(EntityTemplateConfiguration.Flags):
                        flags = JsonSerializer.Deserialize<EntityTemplateFlags>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(EntityTemplateConfiguration.Include):
                        include = JsonSerializer.Deserialize<Key<IEntityTemplate>[]>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(EntityTemplateConfiguration.Components):
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

            return new ResourceResolver<EntityTemplateConfiguration>(() =>
            {
                EntityTemplateConfiguration entityTemplateConfiguration = new()
                {
                    Key = key.Value,
                    Type = type,
                    Flags = flags,
                    Components = components,
                    Include = include,
                };

                return entityTemplateConfiguration;
            });
        }

        public override void Write(Utf8JsonWriter writer, ResourceResolver<EntityTemplateConfiguration> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
