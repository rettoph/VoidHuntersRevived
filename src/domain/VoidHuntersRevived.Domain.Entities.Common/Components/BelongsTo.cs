using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public struct BelongsTo<TOwner, TAs> : IEntityComponent
        where TOwner : unmanaged, IEntityComponent
        where TAs : unmanaged, IEntityComponent
    {
        // Someday, if we want to be able to change EntityId for an existing entity,
        // we should consider adding an UnmanagedReference<IEntityService> in this component
        // and automagically publishing necessary events/actions to make this happen.
        public EntityId OwnerId { get; private set; }

        public BelongsTo(EntityId ownerId)
        {
            this.OwnerId = ownerId;
        }
    }
}
