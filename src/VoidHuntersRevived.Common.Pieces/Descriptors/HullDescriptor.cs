using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components.Instance;
using VoidHuntersRevived.Common.Pieces.Components.Shared;
using VoidHuntersRevived.Common.Pieces.Components.Static;
using VoidHuntersRevived.Common.Pieces.Serialization.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public class HullDescriptor : PieceDescriptor
    {
        public HullDescriptor()
        {
            this.WithInstanceComponents(new ComponentManager[]
            {
                //new ComponentManager<Sockets, SocketsComponentSerializer>(),
                new ComponentManager<Sockets<Location>, SocketLocationsComponentSerializer>(),
                new ComponentManager<Sockets<SocketId>, SocketIdsComponentSerializer>(),
            });
        }

        protected override ResourceColorScheme GetDefaultColorScheme()
        {
            return new ResourceColorScheme(Resources.Colors.HullPrimaryColor, Resources.Colors.HullSecondaryColor);
        }

        protected override zIndex GetZIndex()
        {
            return new zIndex(0);
        }
    }
}
