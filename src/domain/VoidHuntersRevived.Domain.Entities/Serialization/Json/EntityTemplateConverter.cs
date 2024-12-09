using Guppy.Core.Serialization.Common.Services;
using Svelto.ECS;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Serialization.Json
{
    internal sealed class EntityTemplateConverter(
        IPolymorphicJsonSerializerService<IEntityComponent> entityComponentSerializationService
    ) : JsonConverter<EntityTemplateFragment>
    {
        private readonly IPolymorphicJsonSerializerService<IEntityComponent> _entityComponentSerializationService = entityComponentSerializationService;

        public override EntityTemplateFragment? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Key<IEntityTemplate>? key = null;
            EntityTemplateFlags flags = EntityTemplateFlags.None;
            Key<IEntityTemplate>? inherit = null;
            Dictionary<Type, IEntityComponent> components = [];
            List<Type> requiredComponents = [];

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(EntityTemplateFragment.Key):
                        key = JsonSerializer.Deserialize<Key<IEntityTemplate>>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(EntityTemplateFragment.Flags):
                        flags = JsonSerializer.Deserialize<EntityTemplateFlags>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(EntityTemplateFragment.Inherit):
                        inherit = JsonSerializer.Deserialize<Key<IEntityTemplate>>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(EntityTemplateFragment.Components):
                        components = JsonSerializer.Deserialize<Dictionary<Type, IEntityComponent>>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(EntityTemplateFragment.RequiredComponents):
                        requiredComponents.AddRange((JsonSerializer.Deserialize<string[]>(ref reader, options) ?? []).Select(_entityComponentSerializationService.GetType));
                        reader.Read();
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("Unexpected property name {0}", propertyName));
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            if (key is null)
            {
                throw new NotImplementedException();
            }

            EntityTemplateFragment template = new()
            {
                Key = key.Value,
                Flags = flags,
                Inherit = inherit,
                Components = components.Values.ToArray(),
                RequiredComponents = requiredComponents.ToArray()
            };

            return template;
        }

        public override void Write(Utf8JsonWriter writer, EntityTemplateFragment value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
