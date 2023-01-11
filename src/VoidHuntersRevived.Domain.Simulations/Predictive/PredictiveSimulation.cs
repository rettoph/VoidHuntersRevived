using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Resources.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    [GuppyFilter<IGameGuppy>()]
    [SimulationTypeFilter(SimulationType.Predictive)]
    internal sealed class PredictiveSimulation : Simulation<Common.Simulations.Components.Predictive>,
        ISubscriber<Tick>
    {
        private IPredictiveSynchronizationSystem[] _synchronizeSystems;

        public PredictiveSimulation(
            IParallelService simulatedEntities, 
            IGlobalSimulationService globalSimulationService) : base(SimulationType.Predictive, simulatedEntities, globalSimulationService)
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

        public override void PublishEvent(ISimulationData data)
        {
            this.PublishEvent(data, Confidence.Stochastic);
        }

        public void Process(in Tick message)
        {
            foreach(ISimulationData data in message.Data)
            {
                this.PublishEvent(data, Confidence.Deterministic);
            }
        }
    }
}
