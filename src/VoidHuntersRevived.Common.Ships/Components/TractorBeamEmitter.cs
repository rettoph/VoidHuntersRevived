using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Ships.Components
{
    public struct TractorBeamEmitter : IEntityComponent
    {
        public bool Active;
        public EntityId TargetId;
    }
}
