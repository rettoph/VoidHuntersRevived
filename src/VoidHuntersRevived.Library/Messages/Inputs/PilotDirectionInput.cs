using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Messages.Inputs
{
    public class PilotDirectionInput : DirectionInput, ISimulationEvent
    {
        public INetId PilotId { get; }

        public PilotDirectionInput(INetId pilotId, Direction which, bool value) : base(which, value)
        {
            this.PilotId = pilotId;
        }
    }
}
