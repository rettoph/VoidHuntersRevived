using Guppy.Core.Common;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public class EntityTemplateConfiguration
    {
        public required Key<IEntityTemplate> Key { get; init; }
        public required Type? Type { get; init; }
        public EntityTemplateFlags Flags { get; init; }
        public Key<IEntityTemplate>[] Include { get; init; } = Array.Empty<Key<IEntityTemplate>>();

        public HashSet<Type> RequiredComponents { get; init; } = [];
        public required Dictionary<Type, IEntityComponent> Components { get; init; } = [];

        public EntityInitializerDelegate? Initializer { get; init; }

        public static bool CombineConfigurations(Key<IEntityTemplate> key, Dictionary<Key<IEntityTemplate>, EntityTemplateConfiguration[]> dict, [MaybeNullWhen(false)] out EntityTemplateConfiguration result)
        {
            if (dict.TryGetValue(key, out EntityTemplateConfiguration[]? configurations) == false)
            {
                result = null;
                return false;
            }

            EntityTemplateFlags flags = EntityTemplateFlags.None;
            foreach (EntityTemplateConfiguration configuration in configurations)
            {
                flags |= configuration.Flags;
            }

            if (flags.HasFlag(EntityTemplateFlags.Partial))
            {
                // Dont bother creating partial configurations...
                result = null;
                return false;
            }

            Type? type = null;
            HashSet<Key<IEntityTemplate>> included = [];
            HashSet<Type> requiredComponents = [];
            Dictionary<Type, IEntityComponent> components = [];
            EntityInitializerDelegate? initializer = null;

            EntityTemplateConfiguration.PopulateCombineConfigurationValues(
                key, dict,
                ref type, ref included, ref requiredComponents, ref components, ref initializer);

            result = new EntityTemplateConfiguration()
            {
                Key = key,
                Type = type,
                Flags = flags,
                RequiredComponents = requiredComponents,
                Components = components,
                Initializer = initializer
            };

            return true;
        }

        private static void PopulateCombineConfigurationValues(
            Key<IEntityTemplate> key,
            Dictionary<Key<IEntityTemplate>, EntityTemplateConfiguration[]> dict,
            ref Type? type,
            ref HashSet<Key<IEntityTemplate>> included,
            ref HashSet<Type> requiredComponents,
            ref Dictionary<Type, IEntityComponent> components,
            ref EntityInitializerDelegate? initializer)
        {
            if (included.Add(key) == false)
            {
                return;
            }

            if (dict.TryGetValue(key, out EntityTemplateConfiguration[]? configurations) == false)
            {
                return;
            }

            foreach (EntityTemplateConfiguration configuration in configurations)
            {
                // Recersively load included values first...
                foreach (Key<IEntityTemplate> includeKey in configuration.Include)
                {
                    EntityTemplateConfiguration.PopulateCombineConfigurationValues(includeKey, dict,
                        ref type, ref included, ref requiredComponents, ref components, ref initializer);
                }

                foreach (Type requiredType in configuration.RequiredComponents)
                {
                    ThrowIf.Type.IsNotAssignableFrom<IEntityComponent>(requiredType);
                    ThrowIf.Type.IsNotUnmanagedStruct(requiredType);

                    requiredComponents.Add(requiredType);
                }

                // Add components
                components.Merge(configuration.Components);

                if (configuration.Type is null || configuration.Type == type)
                {
                    continue;
                }

                if (type is null || configuration.Type.IsAssignableTo(type))
                {
                    type = configuration.Type;
                }

                // Incompatible types...
                ThrowIf.Type.IsNotAssignableFrom(configuration.Type, type);

                initializer += configuration.Initializer;
            }
        }
    }
}
