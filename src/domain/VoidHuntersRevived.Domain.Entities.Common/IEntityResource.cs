using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public interface IEntityResource<T>
    {
        Id<T> Id { get; }
    }
}
