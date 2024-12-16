using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.FixedPoint.Utilities;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public readonly struct NodeSocket(NodeSocketLocalId localId, Node node, Socket socket)
    {
        public readonly NodeSocketLocalId LocalId = localId;

        public readonly Node Node = node;
        public readonly Socket Socket = socket;

        public FixMatrix LocalTransformation => FixMatrixHelper.FastMultiplyTransformations(this.Socket.Location.Transformation, Node.LocalLocation.Transformation);
        public FixMatrix Transformation => FixMatrixHelper.FastMultiplyTransformations(this.Socket.Location.Transformation, Node.Transformation);

        public NodeSocket(Node node, byte index, Socket socket) : this(new NodeSocketLocalId(node.Id.ToLocalEntityId(), index), node, socket)
        {
        }
    }
}
