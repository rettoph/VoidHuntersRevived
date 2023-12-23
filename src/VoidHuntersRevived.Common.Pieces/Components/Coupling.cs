using Svelto.ECS;

namespace VoidHuntersRevived.Common.Pieces.Components
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
