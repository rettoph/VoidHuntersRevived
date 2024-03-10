using Svelto.ECS;

namespace VoidHuntersRevived.Common.Pieces.Components.Instance
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
