using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Game.Components;

namespace VoidHuntersRevived.Game.Descriptors
{
    public class ShipDescriptor : TreeDescriptor
    {
        public ShipDescriptor()
        {
            this.ExtendWith(new IComponentBuilder[]
            {
                new ComponentBuilder<Helm>(),
                new ComponentBuilder<Tactical>(),
                new ComponentBuilder<TractorBeamEmitter>()
            });
        }
    }
}
