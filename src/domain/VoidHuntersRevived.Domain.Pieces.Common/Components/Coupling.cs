using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components
{
    public readonly struct Coupling(NodeSocketLocalId socketId) : IEntityComponent
    {
        public readonly NodeSocketLocalId SocketId = socketId;
    }
}