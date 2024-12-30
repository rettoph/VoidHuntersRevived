using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Components
{
    public struct TractorBeamEmitter(EntityLocalId localId) : IEntityComponent, IHasMany<Tractorable>
    {
        public bool Active = false;

        public readonly EntityFilterId<Tractorable> TractorableFilterId = EntityFilterId<Tractorable>.Create<TractorBeamEmitter>(localId);
        EntityFilterId<Tractorable> IHasMany<Tractorable>.ChildrenFilterId => this.TractorableFilterId;
    }
}
