using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public readonly struct NodeSocketGlobalId(EntityGlobalId nodeGlobalId, byte socketIndex)
    {
        public readonly EntityGlobalId NodeGlobalId = nodeGlobalId;
        public readonly byte SocketIndex = socketIndex;

        public override bool Equals(object? obj) => obj is NodeSocketGlobalId id &&
                   this.NodeGlobalId.Equals(id.NodeGlobalId) &&
                   this.SocketIndex == id.SocketIndex;

        public override int GetHashCode() => HashCode.Combine(this.NodeGlobalId, this.SocketIndex);

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