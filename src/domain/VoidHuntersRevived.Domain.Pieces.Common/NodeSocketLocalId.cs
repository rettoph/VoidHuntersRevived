using System.Runtime.InteropServices;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public readonly struct NodeSocketLocalId(EntityLocalId nodeLocalId, byte socketIndex) : IEquatable<NodeSocketLocalId>
    {
        private static readonly Dictionary<byte, FilterContextID> _filterContexts = [];

        public static readonly NodeSocketLocalId Empty = default!;

        public readonly EntityLocalId NodeLocalId = nodeLocalId;
        public readonly byte SocketIndex = socketIndex;

        public readonly FilterContextID FilterContextId
        {
            get
            {
                ref FilterContextID filterContextId = ref CollectionsMarshal.GetValueRefOrAddDefault(_filterContexts, this.SocketIndex, out bool exists);
                if (!exists)
                {
                    filterContextId = FilterContextID.GetNewContextID();
                }

                return filterContextId;
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is NodeSocketLocalId id && this.Equals(id);
        }

        public bool Equals(NodeSocketLocalId other)
        {
            return EqualityComparer<EntityLocalId>.Default.Equals(this.NodeLocalId, other.NodeLocalId) &&
                   this.SocketIndex == other.SocketIndex;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.NodeLocalId, this.SocketIndex);
        }

        public override string ToString()
        {
            return $"{this.NodeLocalId}:{this.SocketIndex}";
        }

        public static bool operator ==(NodeSocketLocalId id1, NodeSocketLocalId id2)
        {
            return id1.NodeLocalId.Value == id2.NodeLocalId.Value && id1.SocketIndex == id2.SocketIndex;
        }

        public static bool operator !=(NodeSocketLocalId id1, NodeSocketLocalId id2)
        {
            return id1.NodeLocalId.Value != id2.NodeLocalId.Value || id1.SocketIndex != id2.SocketIndex;
        }
    }
}