using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.FixedPoint.Utilities;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public readonly struct NodeSocket(Node node, NodeSocketId socketId, Socket socket)
    {
        public readonly Node Node = node;

        public readonly NodeSocketId Id = socketId;
        public readonly Socket Socket = socket;

        public FixMatrix LocalTransformation => FixMatrixHelper.FastMultiplyTransformations(this.Socket.Location.Transformation, Node.LocalLocation.Transformation);
        public FixMatrix Transformation => FixMatrixHelper.FastMultiplyTransformations(this.Socket.Location.Transformation, Node.Transformation);

        public NodeSocket(Node node, byte index, Socket socket) : this(node, new NodeSocketId(node.Id, index), socket)
        {
        }
    }
}
