using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;

namespace VoidHuntersRevived.Domain.Pieces.Common.Descriptors
{
    [AutoLoad]
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

        protected override zIndex GetZIndex()
        {
            return new zIndex(0);
        }
    }
}
