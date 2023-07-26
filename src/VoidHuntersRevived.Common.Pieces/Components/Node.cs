using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct Node : IEntityComponent
    {
        public readonly EntityId Id;
        public readonly EntityId TreeId;

        public FixMatrix Transformation;

        public Node(EntityId id, EntityId treeId)
        {
            this.Id = id;
            this.TreeId = treeId;
        }
    }
}
