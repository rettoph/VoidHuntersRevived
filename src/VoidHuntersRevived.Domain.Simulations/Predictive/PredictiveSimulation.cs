using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Identity;
using Guppy.Resources.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public override void Input(ParallelKey user, IData data)
        {
            if (Simulation.Input.Factory.TryCreate(SimulationType.Predictive, user, data, this, out IEvent? instance))
            {
                this.PublishEvent(instance);
            }
        }

        public void Process(in Tick message)
        {
            foreach(UserInput input in message.Inputs)
            {
                if (Simulation.Input.Factory.TryCreate(SimulationType.Lockstep, input.User, input.Data, this, out IEvent? instance))
                {
                    this.PublishEvent(instance);
                }
            }
        }
    }
}
