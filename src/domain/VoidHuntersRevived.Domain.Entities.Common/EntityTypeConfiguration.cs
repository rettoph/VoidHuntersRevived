using Guppy.Core.Common;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public class EntityTypeConfiguration
    {
        public required Key<IEntityType> Key { get; init; }
        public required Type? Type { get; init; }
        public EntityTypeFlags Flags { get; init; }
        public Key<IEntityType>[] Include { get; init; } = Array.Empty<Key<IEntityType>>();

        public HashSet<Type> RequiredComponents { get; init; } = [];
        public required Dictionary<Type, IEntityComponent> Components { get; init; } = [];

        public EntityInitializerDelegate? Initializer { get; init; }

        public static bool CombineConfigurations(Key<IEntityType> key, Dictionary<Key<IEntityType>, EntityTypeConfiguration[]> dict, [MaybeNullWhen(false)] out EntityTypeConfiguration result)
        {
            if (dict.TryGetValue(key, out EntityTypeConfiguration[]? configurations) == false)
            {
                result = null;
                return false;
            }

            EntityTypeFlags flags = EntityTypeFlags.None;
            foreach (EntityTypeConfiguration configuration in configurations)
            {
                flags |= configuration.Flags;
            }

            if (flags.HasFlag(EntityTypeFlags.Partial))
            {
                // Dont bother creating partial configurations...
                result = null;
                return false;
            }

            Type? type = null;
            HashSet<Key<IEntityType>> included = [];
            HashSet<Type> requiredComponents = [];
            Dictionary<Type, IEntityComponent> components = [];
            EntityInitializerDelegate? initializer = null;

            EntityTypeConfiguration.PopulateCombineConfigurationValues(
                key, dict,
                ref type, ref included, ref requiredComponents, ref components, ref initializer);

            result = new EntityTypeConfiguration()
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
            Key<IEntityType> key,
            Dictionary<Key<IEntityType>, EntityTypeConfiguration[]> dict,
            ref Type? type,
            ref HashSet<Key<IEntityType>> included,
            ref HashSet<Type> requiredComponents,
            ref Dictionary<Type, IEntityComponent> components,
            ref EntityInitializerDelegate? initializer)
        {
            if (included.Add(key) == false)
            {
                return;
            }

            if (dict.TryGetValue(key, out EntityTypeConfiguration[]? configurations) == false)
            {
                return;
            }

            foreach (EntityTypeConfiguration configuration in configurations)
            {
                // Recersively load included values first...
                foreach (Key<IEntityType> includeKey in configuration.Include)
                {
                    EntityTypeConfiguration.PopulateCombineConfigurationValues(includeKey, dict,
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
