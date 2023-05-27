using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Identity;
using Guppy.Resources.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Serilog;
using Serilog.Core;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Physics.Factories;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;
using VoidHuntersRevived.Domain.Simulations.Predictive.Services;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    [GuppyFilter<IGameGuppy>()]
    [SimulationTypeFilter(SimulationType.Predictive)]
    internal sealed class PredictiveSimulation : Simulation<Common.Simulations.Components.Predictive>,
        ISubscriber<INetIncomingMessage<Tick>>,
        ISubscriber<INetIncomingMessage<StateTick>>,
        ISubscriber<ISimulationEvent>
    {
        private readonly ILogger _logger;
        private readonly ISimulationEventPublishingService _events;
        private IPredictiveSynchronizationSystem[] _synchronizeSystems;
        private readonly SimulationEventPredictionService _predictionService;
        private readonly Queue<SimulationEventData> _predictions;
        private readonly Queue<ISimulationEvent> _verified;
        private ISimulation _lockstep;

        public PredictiveSimulation(
            ILogger logger,
            IParallelEntityService parallelables,
            ISpaceFactory spaceFactory,
            IGlobalSimulationService globalSimulationService,
            ISimulationEventPublishingService events) : base(SimulationType.Predictive, parallelables, spaceFactory, globalSimulationService)
        {
            _logger = logger;
            _events = events;
            _synchronizeSystems = Array.Empty<IPredictiveSynchronizationSystem>();
            _predictionService = new SimulationEventPredictionService(logger, _events);
            _predictions = new Queue<SimulationEventData>();
            _verified = new Queue<ISimulationEvent>();
        }

        public override void Initialize(IServiceProvider provider)
        {
            base.Initialize(provider);

            _synchronizeSystems = provider.GetRequiredService<IFiltered<IPredictiveSynchronizationSystem>>().Instances.ToArray();
            _lockstep = provider.GetRequiredService<ISimulationService>().First(SimulationType.Lockstep) ?? this;
        }

        protected override void Update(GameTime gameTime)
        {
            Fix64 damping = (Fix64)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (IPredictiveSynchronizationSystem synchronizeSystem in _synchronizeSystems)
            {
                synchronizeSystem.Synchronize(this, _lockstep, gameTime, damping);
            }

            this.UpdateSystems(gameTime);

            while (_predictions.TryDequeue(out SimulationEventData? data))
            {
                this.Publish(data);
            }

            while (_verified.TryDequeue(out ISimulationEvent? verified))
            {
                _predictionService.Verify(this, verified);
            }

            _predictionService.Prune();
        }

        public override ISimulationEvent Publish(SimulationEventData data)
        {
            return _predictionService.Predict(this, data).Event;
        }

        public override void Enqueue(SimulationEventData data)
        {
            _predictions.Enqueue(data);
        }

        public void Process(in INetIncomingMessage<Tick> message)
        {
            foreach(SimulationEventData input in message.Body.Events)
            {
                // this.Enqueue(input);
            }
        }

        public void Process(in INetIncomingMessage<StateTick> message)
        {
            foreach (SimulationEventData input in message.Body.Tick.Events)
            {
                // this.Enqueue(input);
            }
        }

        public void Process(in ISimulationEvent message)
        {
            if(message.Simulation == this)
            { // Ignore events that were already publiblished to the predictive simulation
                return;
            }

            _verified.Enqueue(message);
        }
    }
}
