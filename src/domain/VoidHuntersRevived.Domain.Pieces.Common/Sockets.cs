using Svelto.DataStructures;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components.Instance;

namespace VoidHuntersRevived.Common.Pieces
{
    public ref struct Sockets
    {
        private readonly Sockets<SocketId> _ids;
        private readonly Sockets<Location> _locations;

        public ref Node Node;

        public Sockets(ref Node node, Sockets<SocketId> socketIds, Sockets<Location> socketLocations)
        {
            Node = ref node;

            _ids = socketIds;
            _locations = socketLocations;
        }

        public Sockets(uint index, NB<Node> nodes, NB<Sockets<SocketId>> socketIds, NB<Sockets<Location>> socketLocations)
            : this(ref nodes[index], socketIds[index], socketLocations[index])
        {
        }

        public int Count => _ids.Items.count;

        public Socket this[byte index] => new Socket(this.Node, _ids.Items[index], _locations.Items[index]);
        public Socket this[uint index] => new Socket(this.Node, _ids.Items[index], _locations.Items[index]);
        public Socket this[int index] => new Socket(this.Node, _ids.Items[index], _locations.Items[index]);
    }
}
