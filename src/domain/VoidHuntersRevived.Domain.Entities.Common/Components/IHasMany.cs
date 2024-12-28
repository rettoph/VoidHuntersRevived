using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public interface IHasMany<TChildren> : IEntityComponent
        where TChildren : unmanaged, IEntityComponent
    {
        EntityFilterId<TChildren> ChildrenFilterId { get; }
    }
}
