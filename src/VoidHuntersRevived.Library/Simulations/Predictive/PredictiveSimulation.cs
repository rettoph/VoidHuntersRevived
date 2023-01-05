using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Attributes;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Services;

namespace VoidHuntersRevived.Library.Simulations.Predictive
{
    [SimulationTypeFilter(SimulationTypes.Predictive)]
    internal sealed class PredictiveSimulation : Simulation
    {
        public PredictiveSimulation(IParallelService simulatedEntities) : base(SimulationType.Predictive, simulatedEntities)
        {
        }

        protected override void Update(GameTime gameTime)
        {
            this.UpdateSystems(gameTime);
            this.SynchronizeSystems(gameTime);
        }
    }
}
