using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public interface IBelongsTo<TParent, TSelf> : IEntityComponent
        where TSelf : unmanaged, IBelongsTo<TParent, TSelf>, IEntityComponent
        where TParent : unmanaged, IEntityComponent
    {
        FilterVhId<TSelf> ParentFilterId { get; }
    }
}
