using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Game.Components;

namespace VoidHuntersRevived.Game.Descriptors
{
    public class ShipDescriptor : TreeDescriptor
    {
        public ShipDescriptor()
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<Helm>(
                    builder: new ComponentBuilder<Helm>(),
                    serializer: ComponentSerializer<Helm>.Default),
                new ComponentManager<Tactical>(
                    builder: new ComponentBuilder<Tactical>(),
                    serializer: ComponentSerializer<Tactical>.Default),
                new ComponentManager<TractorBeamEmitter>(
                    builder: new ComponentBuilder<TractorBeamEmitter>(),
                    serializer: ComponentSerializer<TractorBeamEmitter>.Default)
            });
        }
    }
}
