using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Serialization.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public class HullDescriptor : PieceDescriptor
    {
        public HullDescriptor() : base(Resources.Colors.HullPrimaryColor, Resources.Colors.HullSecondaryColor, 0)
        {
            this.WithInstanceComponents(new ComponentManager[]
            {
                //new ComponentManager<Sockets, SocketsComponentSerializer>(),
                new ComponentManager<Sockets<Location>, SocketLocationsComponentSerializer>(),
                new ComponentManager<Sockets<SocketId>, SocketIdsComponentSerializer>(),
            });
        }
    }
}
