using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Game.Pieces.Components;

namespace VoidHuntersRevived.Game.Descriptors
{
    public class TreeDescriptor : VoidHuntersEntityDescriptor
    {
        public TreeDescriptor()
        {
            this.ExtendWith(new IComponentBuilder[]
            {
                new ComponentBuilder<Body>(),
                new ComponentBuilder<Tree>()
            });
        }
    }
}
