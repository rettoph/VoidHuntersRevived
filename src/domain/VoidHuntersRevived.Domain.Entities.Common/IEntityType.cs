using VoidHuntersRevived.Domain.Entities.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public interface IEntityType : IEntityResource<IEntityType>
    {
        VoidHuntersEntityDescriptor Descriptor { get; }
        string Key { get; }
    }

    public interface IEntityType<out T> : IEntityType
        where T : VoidHuntersEntityDescriptor
    {
        new T Descriptor { get; }
    }
}
