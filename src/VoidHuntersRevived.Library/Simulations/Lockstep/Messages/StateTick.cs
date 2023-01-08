using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Messages
{
    internal sealed class StateTick : Message<StateTick>
    {
        public required Tick Tick { get; init; }
    }
}
