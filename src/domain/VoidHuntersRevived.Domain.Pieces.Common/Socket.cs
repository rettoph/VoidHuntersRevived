using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.FixedPoint.Utilities;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Common
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
