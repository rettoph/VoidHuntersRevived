using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Simulations.EventData.Inputs
{
    public class PilotDirectionInput : DirectionInput, ISimulationEventData
    {
        public SimulatedId PilotId { get; }

        public PilotDirectionInput(SimulatedId pilotId, Direction which, bool value) : base(which, value)
        {
            this.PilotId = pilotId;
        }
    }
}
