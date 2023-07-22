using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces
{
    public readonly ref struct NodeJoint
    {
        public readonly ref Node Node;
        public readonly ref Joint Joint;

        public FixMatrix Transformation => Joint.Location.Transformation * Node.Transformation;

        public NodeJoint(ref Node node, ref Joint joint)
        {
            Node = ref node;
            Joint = ref joint;
        }
    }
}
