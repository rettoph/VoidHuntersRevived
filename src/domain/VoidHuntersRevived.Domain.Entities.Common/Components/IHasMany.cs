using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public interface IHasMany<TChildren> : IEntityComponent
        where TChildren : unmanaged, IEntityComponent
    {
        EntityFilterId<TChildren> ChildrenFilterId { get; }
    }
}
