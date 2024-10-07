using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components
{
    public readonly struct Coupling(NodeSocketId socketId) : IEntityComponent
    {
        public readonly NodeSocketId SocketId = socketId;
    }
}
