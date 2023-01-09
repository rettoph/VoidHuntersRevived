using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Messages
{
    internal sealed class StateEnd : Message<StateEnd>
    {
        public required int LastTickId { get; init; }
    }
}
