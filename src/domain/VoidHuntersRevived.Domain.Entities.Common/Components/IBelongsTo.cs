using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public interface IBelongsTo<TOwner, TSelf> : IEntityComponent
        where TSelf : unmanaged, IBelongsTo<TOwner, TSelf>, IEntityComponent
        where TOwner : unmanaged, IEntityComponent
    {
        FilterVhId<TSelf> OwnerFilterId { get; }
    }
}
