using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityType
    {
        Id<IEntityType> Id { get; }
        string Key { get; }
        public EntityTypeFlags Flags { get; }
        VoidHuntersEntityDescriptor Descriptor { get; }
        IEntityType? BaseType { get; }

        IReadOnlyDictionary<Type, IEntityComponent> InstanceComponents { get; }
        IReadOnlyDictionary<Type, IEntityComponent> Components { get; }
    }

    public interface IEntityType<out T> : IEntityType
        where T : VoidHuntersEntityDescriptor
    {
        new T Descriptor { get; }
    }
}
