using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public struct SocketVhId
    {
        public readonly VhId NodeVhId;
        public readonly byte Index;

        public SocketVhId(VhId nodeVhId, byte index)
        {
            NodeVhId = nodeVhId;
            Index = index;
        }

        public override bool Equals(object? obj)
        {
            return obj is SocketVhId id &&
                   NodeVhId.Equals(id.NodeVhId) &&
                   Index == id.Index;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NodeVhId, Index);
        }

        public static bool operator ==(SocketVhId socketVhId1, SocketVhId socketVhId2)
        {
            return socketVhId1.NodeVhId == socketVhId2.NodeVhId && socketVhId1.Index == socketVhId2.Index;
        }

        public static bool operator !=(SocketVhId socketVhId1, SocketVhId socketVhId2)
        {
            return socketVhId1.NodeVhId != socketVhId2.NodeVhId || socketVhId1.Index != socketVhId2.Index;
        }
    }
}
