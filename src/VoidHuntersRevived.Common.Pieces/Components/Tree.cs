using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct Tree : IEntityComponent
    {
        public readonly VhId HeadVhId;
        public FixMatrix Transformation;

        public Tree(VhId headId)
        {
            this.HeadVhId = headId;
        }
    }
}
