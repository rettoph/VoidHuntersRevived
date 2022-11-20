using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Messages.Inputs
{
    public class PilotDirectionInput : DirectionInput, ITickData
    {
        public readonly INetId PilotId;

        public PilotDirectionInput(INetId pilotId, Direction which, bool value) : base(which, value)
        {
            this.PilotId = pilotId;
        }
    }
}
