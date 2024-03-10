using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components.Instance
{
    public struct Coupling : IEntityComponent
    {
        public readonly SocketId SocketId;

        public Coupling(SocketId socketId)
        {
            SocketId = socketId;
        }
    }
}
