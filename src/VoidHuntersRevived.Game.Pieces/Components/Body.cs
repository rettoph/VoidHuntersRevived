using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Pieces.Components
{
    public struct Body : IEntityComponent
    {
        public VhId Id;
        public FixVector2 Position;
        public Fix64 Rotation;
    }
}
