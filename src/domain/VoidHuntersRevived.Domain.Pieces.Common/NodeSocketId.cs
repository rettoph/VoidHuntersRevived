using Svelto.ECS;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public struct NodeSocketId(EntityId nodeId, byte index) : IEquatable<NodeSocketId>
    {
        private static Dictionary<byte, FilterContextID> _filterContexts = new Dictionary<byte, FilterContextID>();

        public static readonly NodeSocketId Empty = default!;

        public readonly EntityId NodeId = nodeId;
        public readonly byte Index = index;

        public readonly FilterContextID FilterContextId
        {
            get
            {
                ref FilterContextID filterContextId = ref CollectionsMarshal.GetValueRefOrAddDefault(_filterContexts, this.Index, out bool exists);
                if (!exists)
                {
                    filterContextId = FilterContextID.GetNewContextID();
                }

                return filterContextId;
            }
        }

        public SocketVhId VhId => new SocketVhId(this.NodeId.VhId, Index);

        public override bool Equals(object? obj)
        {
            return obj is NodeSocketId id && Equals(id);
        }

        public bool Equals(NodeSocketId other)
        {
            return EqualityComparer<EntityId>.Default.Equals(this.NodeId, other.NodeId) &&
                   this.Index == other.Index &&
                   EqualityComparer<SocketVhId>.Default.Equals(this.VhId, other.VhId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.NodeId, this.Index, this.VhId);
        }

        public static bool operator ==(NodeSocketId id1, NodeSocketId id2)
        {
            return id1.NodeId.VhId.Value == id2.NodeId.VhId.Value && id1.Index == id2.Index;
        }

        public static bool operator !=(NodeSocketId id1, NodeSocketId id2)
        {
            return id1.NodeId.VhId.Value != id2.NodeId.VhId.Value || id1.Index != id2.Index;
        }
    }
}
