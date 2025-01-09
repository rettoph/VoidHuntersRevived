using Svelto.DataStructures;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public ref struct NodeSockets
    {
        private readonly Sockets _sockets;

        public readonly EntityLocalId BodyLocalId;
        public ref Node Node;
        public ref Fixture Fixture;

        public NodeSockets(EntityLocalId bodyLocalId, ref Node node, ref Fixture fixture, Sockets sockets)
        {
            this.BodyLocalId = bodyLocalId;
            this.Node = ref node;
            this.Fixture = ref fixture;

            this._sockets = sockets;
        }

        public NodeSockets(EntityLocalId bodyLocalId, uint index, NB<Node> nodes, NB<Fixture> fixtures, NB<Sockets> sockets)
            : this(bodyLocalId, ref nodes[index], ref fixtures[index], sockets[index])
        {
        }

        public readonly int Count => this._sockets.Items.count;

        public readonly NodeSocket this[byte index] => new(this.BodyLocalId, this.Node, this.Fixture, index, this._sockets.Items[index]);
        public readonly NodeSocket this[uint index] => new(this.BodyLocalId, this.Node, this.Fixture, (byte)index, this._sockets.Items[index]);
        public readonly NodeSocket this[int index] => new(this.BodyLocalId, this.Node, this.Fixture, (byte)index, this._sockets.Items[index]);
    }
}