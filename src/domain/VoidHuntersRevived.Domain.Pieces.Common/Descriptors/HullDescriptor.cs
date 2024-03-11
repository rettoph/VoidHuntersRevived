using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Shared;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;

namespace VoidHuntersRevived.Domain.Pieces.Common.Descriptors
{
    public class HullDescriptor : PieceDescriptor
    {
        public HullDescriptor()
        {
            this.WithInstanceComponents([
                //new ComponentManager<Sockets, SocketsComponentSerializer>(),
                new ComponentBuilder<Sockets<Location>>(),
                new ComponentBuilder<Sockets<SocketId>>(),
            ]);
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
