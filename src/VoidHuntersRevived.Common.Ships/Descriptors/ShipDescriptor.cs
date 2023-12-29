using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Physics.Serialization.Components;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Ships.Serialization.Components;

namespace VoidHuntersRevived.Common.Ships.Descriptors
{
    public abstract class ShipDescriptor : TreeDescriptor
    {
        public ShipDescriptor()
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<PhysicsBubble, PhysicsBubbleComponentSerializer>(),
                new ComponentManager<Helm, HelmComponentSerializer>(),
                new ComponentManager<Tactical, TacticalComponentSerializer>(),
                new ComponentManager<TractorBeamEmitter, TractorBeamEmitterComponentSerializer>()
            });
        }
    }
}
