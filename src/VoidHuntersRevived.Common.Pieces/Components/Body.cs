using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct Body : IEntityComponent
    {
        public FixVector2 Position;
        public Fix64 Rotation;
    }
}
