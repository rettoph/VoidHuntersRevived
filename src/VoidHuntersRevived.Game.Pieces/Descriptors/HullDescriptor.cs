using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Game.Pieces.Resources;

namespace VoidHuntersRevived.Game.Pieces.Descriptors
{
    public class HullDescriptor : PieceDescriptor
    {
        public HullDescriptor()
        {
            this.ExtendWith(new IComponentBuilder[]
            {
                new ComponentBuilder<ResourceId<Visible>>(),
                new ComponentBuilder<ResourceId<Rigid>>()
            });
        }
    }
}
