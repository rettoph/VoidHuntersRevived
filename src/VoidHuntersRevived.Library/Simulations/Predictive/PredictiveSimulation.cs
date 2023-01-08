using Guppy.Common;
using Guppy.Resources.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Library.Constants;

namespace VoidHuntersRevived.Library.Simulations.Predictive
{
    [SimulationTypeFilter(SimulationType.Predictive)]
    internal sealed class PredictiveSimulation : Simulation<Common.Simulations.Components.Predictive>
    {
        private IPredictiveSynchronizationSystem[] _synchronizeSystems;

        public PredictiveSimulation(IParallelService simulatedEntities) : base(SimulationType.Predictive, simulatedEntities)
        {
            _synchronizeSystems = Array.Empty<IPredictiveSynchronizationSystem>();
        }

        public override void Initialize(IServiceProvider provider)
        {
            base.Initialize(provider);

            _synchronizeSystems = provider.GetRequiredService<IFiltered<IPredictiveSynchronizationSystem>>().Instances.ToArray();
        }

        protected override void Update(GameTime gameTime)
        {
            this.UpdateSystems(gameTime);

            var damping = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (IPredictiveSynchronizationSystem synchronizeSystem in _synchronizeSystems)
            {
                synchronizeSystem.Synchronize(this, gameTime, damping);
            }
        }
    }
}
