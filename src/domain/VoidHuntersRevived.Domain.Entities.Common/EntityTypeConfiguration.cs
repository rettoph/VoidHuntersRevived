using Guppy.Core.Common;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public class EntityTypeConfiguration
    {
        public required string Name { get; init; }
        public required Type? Type { get; init; }
        public EntityTypeFlags Flags { get; init; }
        public string[] Include { get; init; } = Array.Empty<string>();

        public HashSet<Type> RequiredComponents { get; init; } = new HashSet<Type>();
        public required Dictionary<Type, IEntityComponent> Components { get; init; } = new Dictionary<Type, IEntityComponent>();

        public EntityInitializerDelegate? Initializer { get; init; }
        public EntityDisposerDelegate? Disposer { get; init; }

        public static bool CombineConfigurations(string name, Dictionary<string, EntityTypeConfiguration[]> dict, [MaybeNullWhen(false)] out EntityTypeConfiguration result)
        {
            if (dict.TryGetValue(name, out EntityTypeConfiguration[]? configurations) == false)
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
            HashSet<string> included = new HashSet<string>();
            HashSet<Type> requiredComponents = new HashSet<Type>();
            Dictionary<Type, IEntityComponent> components = new Dictionary<Type, IEntityComponent>();
            EntityInitializerDelegate? initializer = null;
            EntityDisposerDelegate? disposer = null;

            EntityTypeConfiguration.PopulateCombineConfigurationValues(
                name, dict,
                ref type, ref included, ref requiredComponents, ref components, ref initializer, ref disposer);

            result = new EntityTypeConfiguration()
            {
                Name = name,
                Type = type,
                Flags = flags,
                RequiredComponents = requiredComponents,
                Components = components,
                Initializer = initializer,
                Disposer = disposer
            };

            return true;
        }

        private static void PopulateCombineConfigurationValues(
            string name,
            Dictionary<string, EntityTypeConfiguration[]> dict,
            ref Type? type,
            ref HashSet<string> included,
            ref HashSet<Type> requiredComponents,
            ref Dictionary<Type, IEntityComponent> components,
            ref EntityInitializerDelegate? initializer,
            ref EntityDisposerDelegate? disposer)
        {
            if (included.Add(name) == false)
            {
                return;
            }

            if (dict.TryGetValue(name, out EntityTypeConfiguration[]? configurations) == false)
            {
                return;
            }

            foreach (EntityTypeConfiguration configuration in configurations)
            {
                // Recersively load included values first...
                foreach (string includeKey in configuration.Include)
                {
                    EntityTypeConfiguration.PopulateCombineConfigurationValues(includeKey, dict,
                        ref type, ref included, ref requiredComponents, ref components, ref initializer, ref disposer);
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
                disposer += configuration.Disposer;
            }
        }
    }
}
