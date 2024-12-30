using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public readonly struct NodeSocket(EntityLocalId bodyLocalId, NodeSocketLocalId localId, Node node, Fixture fixture, Socket socket)
    {
        public readonly EntityLocalId BodyLocalId = bodyLocalId;
        public readonly NodeSocketLocalId LocalId = localId;

        public readonly Node Node = node;
        public readonly Fixture Fixture = fixture;
        public readonly Socket Socket = socket;

        public FixTransform2D LocalTransform => this.Socket.NodeTransform * this.Fixture.LocalTransform;
        public FixTransform2D WorldTransform => this.Socket.NodeTransform * this.Fixture.WorldTransform;

        public NodeSocket(EntityLocalId bodyLocalId, Node node, Fixture fixture, byte index, Socket socket) : this(bodyLocalId, new NodeSocketLocalId(node.LocalId, index), node, fixture, socket)
        {
        }
    }
}
