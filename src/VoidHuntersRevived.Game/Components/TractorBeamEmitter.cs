using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Game.Components
{
    public struct TractorBeamEmitter : IEntityComponent
    {
        public bool Active;
        public VhId TargetId;
    }
}
