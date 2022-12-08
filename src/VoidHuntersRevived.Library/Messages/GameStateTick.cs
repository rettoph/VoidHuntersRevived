using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Messages
{
    public sealed class GameStateTick : Message
    {
        public readonly Tick Tick;

        public GameStateTick(Tick tick)
        {
            this.Tick = tick;
        }
    }
}
