using Guppy.Resources;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Serialization.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public abstract class PieceDescriptor : VoidHuntersEntityDescriptor
    {
        public PieceDescriptor(Resource<Color> primaryColor, Resource<Color> secondaryColor, int order) : base(primaryColor, secondaryColor, order)
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<Plug, PlugComponentSerializer>(in Plug.Default),
                new ComponentManager<Coupling, CouplingComponentSerializer>(),
                new ComponentManager<Node, NodeComponentSerializer>(),
                new ComponentManager<Rigid, RigidComponentSerializer>(),
                new ComponentManager<Visible, VisibleComponentSerializer>(),
            });
        }
    }
}
