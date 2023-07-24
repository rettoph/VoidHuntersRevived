using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces
{
    public readonly ref struct NodeJoint
    {
        public readonly NodeJointId Id;
        public readonly ref Node Node;
        public readonly ref Joint Joint;

        public FixMatrix Transformation => Joint.Location.Transformation * Node.Transformation;

        public NodeJoint(ref Node node, ref Joint joint)
        {
            this.Id = new NodeJointId(node.Id.VhId, joint.Index);
            Node = ref node;
            Joint = ref joint;
        }
    }
}
