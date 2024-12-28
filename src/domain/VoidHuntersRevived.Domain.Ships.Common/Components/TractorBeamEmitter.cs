using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Ships.Common.Components
{
    public struct TractorBeamEmitter : IEntityComponent
    {
        public bool Active;

        public TractorBeamEmitter()
        {
            this.Active = false;
        }
    }
}
