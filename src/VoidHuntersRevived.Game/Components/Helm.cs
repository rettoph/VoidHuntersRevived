using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Game.Enums;

namespace VoidHuntersRevived.Game.Components
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
