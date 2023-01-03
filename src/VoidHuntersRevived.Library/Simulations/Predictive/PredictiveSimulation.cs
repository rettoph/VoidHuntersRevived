using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Attributes;
using VoidHuntersRevived.Common.Services;

namespace VoidHuntersRevived.Library.Simulations.Predictive
{
    [SimulationTypeFilter(SimulationType.Predictive)]
    internal sealed class PredictiveSimulation : Simulation
    {
        public PredictiveSimulation(ISimulatedService simulatedEntities) : base(SimulationType.Predictive, simulatedEntities)
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }
    }
}
