using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Ships.Serialization.Components;

namespace VoidHuntersRevived.Common.Ships.Descriptors
{
    public class ShipDescriptor : TreeDescriptor
    {
        public ShipDescriptor()
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<Helm, HelmComponentSerializer>(),
                new ComponentManager<Tactical, TacticalComponentSerializer>(),
                new ComponentManager<TractorBeamEmitter, TractorBeamEmitterComponentSerializer>()
            });
        }
    }
}
