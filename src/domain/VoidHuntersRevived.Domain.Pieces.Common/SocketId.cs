using Svelto.ECS;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public struct SocketId : IEquatable<SocketId>
    {
        private static Dictionary<byte, FilterContextID> _filterContexts = new Dictionary<byte, FilterContextID>();

        public static readonly SocketId Empty = default!;

        public readonly EntityId NodeId;
        public readonly byte Index;

        public readonly FilterContextID FilterContextId
        {
            get
            {
                ref FilterContextID filterContextId = ref CollectionsMarshal.GetValueRefOrAddDefault(_filterContexts, Index, out bool exists);
                if (!exists)
                {
                    filterContextId = FilterContextID.GetNewContextID();
                }

                return filterContextId;
            }
        }

        public SocketVhId VhId => new SocketVhId(NodeId.VhId, Index);

        public SocketId(EntityId nodeId, byte index)
        {
            NodeId = nodeId;
            Index = index;


        }

        public override bool Equals(object? obj)
        {
            return obj is SocketId id && Equals(id);
        }

        public bool Equals(SocketId other)
        {
            return EqualityComparer<EntityId>.Default.Equals(NodeId, other.NodeId) &&
                   Index == other.Index &&
                   EqualityComparer<SocketVhId>.Default.Equals(VhId, other.VhId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NodeId, Index, VhId);
        }

        public static bool operator ==(SocketId id1, SocketId id2)
        {
            return id1.NodeId.VhId.Value == id2.NodeId.VhId.Value && id1.Index == id2.Index;
        }

        public static bool operator !=(SocketId id1, SocketId id2)
        {
            return id1.NodeId.VhId.Value != id2.NodeId.VhId.Value || id1.Index != id2.Index;
        }
    }
}
