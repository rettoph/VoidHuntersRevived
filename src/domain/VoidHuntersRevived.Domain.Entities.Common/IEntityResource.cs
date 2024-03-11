using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public interface IEntityResource<T>
    {
        Id<T> Id { get; }
    }
}
