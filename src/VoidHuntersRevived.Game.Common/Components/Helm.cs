using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Game.Common.Enums;

namespace VoidHuntersRevived.Game.Common.Components
{
    public struct Helm
    {
        public Direction Direction;

        public Helm()
        {
            this.Direction = Direction.None;
        }
    }
}
