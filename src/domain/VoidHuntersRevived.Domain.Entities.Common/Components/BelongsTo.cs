using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public struct BelongsTo<TOwner, TAs>(VhId ownerVhId) : IEntityComponent
        where TOwner : unmanaged, IEntityComponent
        where TAs : unmanaged, IEntityComponent
    {
        // Someday, if we want to be able to change EntityVhId for an existing entity,
        // we should consider adding an UnmanagedReference<IEntityService> in this component
        // and automagically publishing necessary events/actions to make this happen.
        public VhId OwnerVhId { get; private set; } = ownerVhId;
    }
}
