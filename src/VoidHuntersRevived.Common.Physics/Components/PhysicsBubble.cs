using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Physics.Components
{
    public struct PhysicsBubble : IEntityComponent
    {
        public PhysicsBubble(Fix64 radius)
        {
            this.Radius = radius;
        }

        public required bool Enabled { get; init; }
        public required Fix64 Radius { get; init; }
    }
}
