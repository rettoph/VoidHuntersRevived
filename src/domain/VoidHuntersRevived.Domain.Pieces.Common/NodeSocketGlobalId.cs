using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public readonly struct NodeSocketGlobalId(EntityGlobalId nodeGlobalId, byte socketIndex)
    {
        public readonly EntityGlobalId NodeGlobalId = nodeGlobalId;
        public readonly byte SocketIndex = socketIndex;

        public override bool Equals(object? obj)
        {
            return obj is NodeSocketGlobalId id &&
                   NodeGlobalId.Equals(id.NodeGlobalId) &&
                   SocketIndex == id.SocketIndex;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NodeGlobalId, SocketIndex);
        }

        public static bool operator ==(NodeSocketGlobalId socketVhId1, NodeSocketGlobalId socketVhId2)
        {
            return socketVhId1.NodeGlobalId == socketVhId2.NodeGlobalId && socketVhId1.SocketIndex == socketVhId2.SocketIndex;
        }

        public static bool operator !=(NodeSocketGlobalId socketVhId1, NodeSocketGlobalId socketVhId2)
        {
            return socketVhId1.NodeGlobalId != socketVhId2.NodeGlobalId || socketVhId1.SocketIndex != socketVhId2.SocketIndex;
        }
    }
}
