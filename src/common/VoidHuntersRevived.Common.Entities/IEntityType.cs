using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityType
    {
        Id<IEntityType> Id { get; }
        VoidHuntersEntityDescriptor Descriptor { get; }
        string Key { get; }
    }

    public interface IEntityType<out T> : IEntityType
        where T : VoidHuntersEntityDescriptor
    {
        new T Descriptor { get; }
    }
}
