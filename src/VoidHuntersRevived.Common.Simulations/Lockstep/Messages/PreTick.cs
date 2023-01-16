using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Lockstep.Messages
{
    public sealed class PreTick : Message<PreTick>
    {
        public readonly State State;

        internal PreTick(State state)
        {
            State = state;
        }
    }
}
