using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Physics.Common.Serialization.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Serialization.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Descriptors
{
    public abstract class ShipDescriptor : TreeDescriptor
    {
        public ShipDescriptor()
        {
            this.WithInstanceComponents(new ComponentManager[]
            {
                new ComponentManager<PhysicsBubble, PhysicsBubbleComponentSerializer>(),
                new ComponentManager<Helm, HelmComponentSerializer>(),
                new ComponentManager<Tactical, TacticalComponentSerializer>(),
                new ComponentManager<TractorBeamEmitter, TractorBeamEmitterComponentSerializer>()
            });
        }
    }
}
