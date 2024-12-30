using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Components
{
    public readonly struct Tractorable(EntityLocalId tractorBeamEmitterLocalId) : IEntityComponent, IBelongsTo<TractorBeamEmitter, Tractorable>
    {
        public readonly EntityFilterId<Tractorable> TractorBeamEmitterFilterId = EntityFilterId<Tractorable>.Create<TractorBeamEmitter>(tractorBeamEmitterLocalId);
        EntityFilterId<Tractorable> IBelongsTo<TractorBeamEmitter, Tractorable>.ParentFilterId => this.TractorBeamEmitterFilterId;
    }
}
