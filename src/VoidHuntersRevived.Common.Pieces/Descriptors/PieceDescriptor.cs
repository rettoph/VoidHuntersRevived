using Microsoft.Xna.Framework;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Serialization.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public abstract class PieceDescriptor : VoidHuntersEntityDescriptor
    {
        public PieceDescriptor()
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<Plug, PlugComponentSerializer>(new ComponentBuilder<Plug>(in Plug.Default)),
                new ComponentManager<Coupling, CouplingComponentSerializer>(new ComponentBuilder<Coupling>()),
                new ComponentManager<Node, NodeComponentSerializer>(new ComponentBuilder<Node>()),
                new ComponentManager<Rigid, RigidComponentSerializer>(new ComponentBuilder<Rigid>()),
                new ComponentManager<Visible, VisibleComponentSerializer>(new ComponentBuilder<Visible>()),
            });
        }
    }
}
