using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Shared;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;
using VoidHuntersRevived.Domain.Pieces.Common.Serialization.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.Descriptors
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
