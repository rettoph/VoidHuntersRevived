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
    public struct Tree : IEntityComponent
    {
        public readonly EntityId HeadId;
        public FixMatrix Transformation;

        public Tree(EntityId headId)
        {
            this.HeadId = headId;
        }
    }
}
