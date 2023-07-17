using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct Node : IEntityComponent
    {
        public readonly VhId TreeId;
        public FixMatrix Transformation;

        public Node(VhId treeId)
        {
            this.TreeId = treeId;
        }
    }
}
