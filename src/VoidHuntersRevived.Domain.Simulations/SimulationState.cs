using Guppy;
using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Domain.Simulations.Commands;

namespace VoidHuntersRevived.Domain.Simulations
{
    internal sealed class SimulationState : State<SimulationType>
    {
        public readonly ISimulation _simulation;

        public SimulationState(ISimulation simulation)
        {
            _simulation = simulation;
        }

        public override SimulationType GetValue()
        {
            return _simulation.Type;
        }
    }
}
