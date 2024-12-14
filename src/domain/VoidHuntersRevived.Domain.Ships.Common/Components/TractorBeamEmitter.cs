using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Ships.Common.Components
{
    public struct TractorBeamEmitter : IEntityComponent
    {
        public bool Active;
        public EntityId TargetId;

        public TractorBeamEmitter()
        {
            this.Active = false;
        }
    }
}
