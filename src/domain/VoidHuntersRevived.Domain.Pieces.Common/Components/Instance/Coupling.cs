using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components.Instance
{
    public struct Coupling(NodeSocketId socketId) : IEntityComponent
    {
        public readonly NodeSocketId SocketId = socketId;
    }
}
