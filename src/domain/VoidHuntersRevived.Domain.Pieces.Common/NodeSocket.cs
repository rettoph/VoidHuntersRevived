using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.FixedPoint.Utilities;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public readonly struct NodeSocket
    {
        public readonly Node Node;

        public readonly NodeSocketId Id;
        public readonly Socket Socket;

        public FixMatrix LocalTransformation => FixMatrixHelper.FastMultiplyTransformations(this.Socket.Location.Transformation, Node.LocalLocation.Transformation);
        public FixMatrix Transformation => FixMatrixHelper.FastMultiplyTransformations(this.Socket.Location.Transformation, Node.Transformation);

        //public Matrix XnaLocalTransformation => FixMatrixHelper.FastMultiplyTransformationsToXnaMatrix(this.Location.Transformation, Node.Transformation);
        //public Matrix XnaTransformation => FixMatrixHelper.FastMultiplyTransformationsToXnaMatrix(this.Location.Transformation, Node.Transformation);


        public NodeSocket(Node node, NodeSocketId socketId, Socket socket)
        {
            this.Node = node;
            this.Id = socketId;
            this.Socket = socket;
        }

        public NodeSocket(Node node, byte index, Socket socket) : this(node, new NodeSocketId(node.Id, index), socket)
        {
        }
    }
}
