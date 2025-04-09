using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public interface IEntityAsset<T>
    {
        Id<T> Id { get; }
    }
}