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
    internal sealed class PredictiveSimulation : Simulation<Common.Simulations.Components.Predictive>
    {
        private IPredictiveSynchronizationSystem[] _synchronizeSystems;
        private Dictionary<int, Prediction> _predictions;

        public PredictiveSimulation(
            IParallelService simulatedEntities, 
            IGlobalSimulationService globalSimulationService) : base(SimulationType.Predictive, simulatedEntities, globalSimulationService)
        {
            _synchronizeSystems = Array.Empty<IPredictiveSynchronizationSystem>();
            _predictions = new Dictionary<int, Prediction>();
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

        public override void PublishEvent(IData data, DataSource source)
        {
            var predictionId = data.GetHashCode();

            if (source == DataSource.External)
            {
                base.PublishEvent(data, source);
                _predictions.Add(predictionId, new Prediction(predictionId, data));
                return;
            }

            if(_predictions.Remove(predictionId))
            {
                return;
            }

            base.PublishEvent(data, source);
        }

        public override void PublishEvent(IData data)
        {
            this.PublishEvent(data, DataSource.Internal);
        }
    }
}
