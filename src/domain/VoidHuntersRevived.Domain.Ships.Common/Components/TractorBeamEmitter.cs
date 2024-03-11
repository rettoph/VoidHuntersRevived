using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Ships.Common.Components
{
    public struct TractorBeamEmitter : IEntityComponent
    {
        public static FilterContextID TractorableFilterContext = FilterContextID.GetNewContextID();

        public readonly EntityId Id;
        public bool Active;
        public EntityId TargetId;

        public TractorBeamEmitter(EntityId id) : this()
        {
            this.Id = id;
            this.Active = false;
        }
    }
}
