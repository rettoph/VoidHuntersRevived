using VoidHuntersRevived.Common.FixedPoint.Utilities;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces
{
    public readonly struct Socket
    {
        public readonly Node Node;

        public readonly SocketId Id;
        public readonly Location Location;

        public FixMatrix LocalTransformation => FixMatrixHelper.FastMultiplyTransformations(this.Location.Transformation, Node.LocalLocation.Transformation);
        public FixMatrix Transformation => FixMatrixHelper.FastMultiplyTransformations(this.Location.Transformation, Node.Transformation);

        //public Matrix XnaLocalTransformation => FixMatrixHelper.FastMultiplyTransformationsToXnaMatrix(this.Location.Transformation, Node.Transformation);
        //public Matrix XnaTransformation => FixMatrixHelper.FastMultiplyTransformationsToXnaMatrix(this.Location.Transformation, Node.Transformation);


        public Socket(Node node, SocketId socketId, Location location)
        {
            this.Node = node;
            this.Id = socketId;
            this.Location = location;
        }
    }
}
