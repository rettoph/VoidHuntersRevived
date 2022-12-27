using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Simulations.EventData.Inputs;

namespace VoidHuntersRevived.Library.Components
{
    public sealed class Pilotable
    {
        public Direction Direction = Direction.None;

        public void SetDirection(DirectionInput input)
        {
            if (input.Value && (this.Direction & input.Which) == 0)
            {
                this.Direction |= input.Which;
                return;
            }

            if (!input.Value && (this.Direction & input.Which) != 0)
            {
                this.Direction &= ~input.Which;
                return;
            }
        }
    }
}
