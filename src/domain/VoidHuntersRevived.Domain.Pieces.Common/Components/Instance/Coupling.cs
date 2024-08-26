using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components.Instance
{
    public struct Coupling : IEntityComponent
    {
        public readonly NodeSocketId SocketId;

        public Coupling(NodeSocketId socketId)
        {
            SocketId = socketId;
        }
    }
}
