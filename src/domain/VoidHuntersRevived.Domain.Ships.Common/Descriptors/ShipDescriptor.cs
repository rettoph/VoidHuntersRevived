using Svelto.ECS;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;
using VoidHuntersRevived.Domain.Ships.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Descriptors
{
    public abstract class ShipDescriptor : TreeDescriptor
    {
        public ShipDescriptor()
        {
            this.WithInstanceComponents([
                new ComponentBuilder<PhysicsBubble>(),
                new ComponentBuilder<Helm>(),
                new ComponentBuilder<Tactical>(),
                new ComponentBuilder<TractorBeamEmitter>()
            ]);
        }
    }
}
