using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Pieces
{
    public struct Node : IEntityComponent
    {
        public VhId TreeId;
        public FixMatrix Transformation;
    }
}
