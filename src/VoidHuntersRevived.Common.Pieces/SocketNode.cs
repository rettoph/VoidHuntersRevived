using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces
{
    public readonly ref struct SocketNode
    {
        public readonly ref Node Node;
        public readonly ref Socket Joint;

        public FixMatrix Transformation => Joint.Location.Transformation * Node.Transformation;

        public SocketNode(ref Socket joint, ref Node node)
        {
            Node = ref node;
            Joint = ref joint;
        }
    }
}
