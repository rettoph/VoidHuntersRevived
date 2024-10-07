using Svelto.DataStructures;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public ref struct NodeSockets
    {
        private readonly Sockets _sockets;

        public ref Node Node;

        public NodeSockets(ref Node node, Sockets sockets)
        {
            Node = ref node;

            _sockets = sockets;
        }

        public NodeSockets(uint index, NB<Node> nodes, NB<Sockets> sockets)
            : this(ref nodes[index], sockets[index])
        {
        }

        public int Count => _sockets.Items.count;

        public NodeSocket this[byte index] => new NodeSocket(this.Node, index, _sockets.Items[index]);
        public NodeSocket this[uint index] => new NodeSocket(this.Node, (byte)index, _sockets.Items[index]);
        public NodeSocket this[int index] => new NodeSocket(this.Node, (byte)index, _sockets.Items[index]);
    }
}
