using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public interface IBelongsTo<TParent, TSelf> : IEntityComponent
        where TParent : unmanaged, IEntityComponent
        where TSelf : unmanaged, IBelongsTo<TParent, TSelf>, IEntityComponent
    {
        EntityFilterId<TSelf> ParentFilterId { get; }
    }
}