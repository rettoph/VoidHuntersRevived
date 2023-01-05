using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Library.Simulations.Predictive
{
    [SimulationTypeFilter(SimulationType.Predictive)]
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
