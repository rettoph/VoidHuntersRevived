using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

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
    }
}
