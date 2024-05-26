using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common
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
