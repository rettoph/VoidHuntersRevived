using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Ships.Enums;

namespace VoidHuntersRevived.Common.Ships.Components
{
    public struct Helm : IEntityComponent
    {
        public Direction Direction;

        public Helm()
        {
            this.Direction = Direction.None;
        }
    }
}
