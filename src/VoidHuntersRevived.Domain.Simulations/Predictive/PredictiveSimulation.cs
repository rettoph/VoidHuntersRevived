using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Identity;
using Guppy.Resources.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
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
        ISubscriber<INetIncomingMessage<Tick>>
    {
        private readonly ISimulationEventPublishingService _input;
        private IPredictiveSynchronizationSystem[] _synchronizeSystems;
        private readonly IBus _bus;
        private readonly Dictionary<Guid, ISimulationEventData> _events;

        public PredictiveSimulation(
            IBus bus,
            IParallelableService simulatedEntities, 
            IGlobalSimulationService globalSimulationService,
            ISimulationEventPublishingService input) : base(SimulationType.Predictive, simulatedEntities, globalSimulationService)
        {
            _synchronizeSystems = Array.Empty<IPredictiveSynchronizationSystem>();
            _bus = bus;
            _events = new Dictionary<Guid, ISimulationEventData>();
            _input = input;
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

        public override void Publish(ISimulationEventData data)
        {
            ref ISimulationEventData? cached = ref CollectionsMarshal.GetValueRefOrAddDefault(_events, data.Id.Hash, out bool exists);

            if (exists)
            {
                // Indicates a duplicate input.
                // Most likely a previously predicted local input thats
                // been bounced back by the server.
                // No need to re-input within this simulation.
                return;
            }

            cached = data;

            _input.Publish(this, data);
        }

        public override void Input(SimulationInput input)
        {
            this.Publish(input);
        }

        public void Process(in INetIncomingMessage<Tick> message)
        {
            foreach(SimulationInput input in message.Body.Inputs)
            {
                this.Input(input);
            }
        }
    }
}
