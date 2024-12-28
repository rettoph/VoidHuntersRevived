using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public readonly struct NodeSocket(NodeSocketLocalId localId, Node node, Socket socket)
    {
        public readonly NodeSocketLocalId LocalId = localId;

        public readonly Node Node = node;
        public readonly Socket Socket = socket;

        public FixTransform2D LocalTransform => this.Socket.NodeTransform * this.Node.LocalTransformation;
        public FixTransform2D WorldTransform => this.Socket.NodeTransform * this.Node.Transformation;

        public NodeSocket(Node node, byte index, Socket socket) : this(new NodeSocketLocalId(node.LocalId, index), node, socket)
        {
        }
    }
}
