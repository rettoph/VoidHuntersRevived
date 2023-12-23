using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities
{
    public interface IEntityType : IEntityResource<IEntityType>
    {
        VoidHuntersEntityDescriptor Descriptor { get; }
        string Key { get; }

        IEntityTypeConfiguration BuildConfiguration();
    }

    public interface IEntityType<out T> : IEntityType
        where T : VoidHuntersEntityDescriptor
    {
        new T Descriptor { get; }
    }
}
