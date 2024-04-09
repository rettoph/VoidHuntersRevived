using Svelto.ECS;
using System.Collections.ObjectModel;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public sealed class EntityContext
    {
        public readonly Id<EntityContext> Id;
        public readonly string Key;
        public readonly IEntityType<VoidHuntersEntityDescriptor> EntityType;
        public readonly VoidHuntersEntityDescriptor Descriptor;
        public readonly IReadOnlyDictionary<Type, IEntityComponent> InstanceComponents;
        public readonly IReadOnlyDictionary<Type, IEntityComponent> StaticComponents;

        public EntityContext(
            string key,
            VoidHuntersEntityDescriptor descriptor,
            Dictionary<Type, IEntityComponent> components,
            Dictionary<Type, IEntityComponent> data)
        {
            this.Id = HashBuilder<EntityContext, VhId, VhId>.Instance.CalculateId(VhId.HashString(key), descriptor.Id.Value);
            this.Key = key;
            this.EntityType = BuildEntityType(descriptor, key);
            this.Descriptor = descriptor;
            this.InstanceComponents = new ReadOnlyDictionary<Type, IEntityComponent>(components);
            this.StaticComponents = new ReadOnlyDictionary<Type, IEntityComponent>(data);
        }

        private static IEntityType<VoidHuntersEntityDescriptor> BuildEntityType(VoidHuntersEntityDescriptor descriptor, string key)
        {
            Type entityTypeType = typeof(EntityType<>).MakeGenericType(descriptor.GetType());

            return (IEntityType<VoidHuntersEntityDescriptor>)Activator.CreateInstance(entityTypeType, key)!;
        }
    }
}
