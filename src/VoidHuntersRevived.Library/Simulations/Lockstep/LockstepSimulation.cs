using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using LiteNetLib;
using Microsoft.Xna.Framework;
using Serilog;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Library.Simulations.Lockstep.Providers;
using VoidHuntersRevived.Library.Simulations.Lockstep.Services;

namespace VoidHuntersRevived.Library.Simulations.Lockstep
{
    [GuppyFilter<GameGuppy>()]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class LockstepSimulation : Simulation<Common.Simulations.Components.Lockstep>, ISimulation,
        ILockstepSimulation,
        ISubscriber<Tick>,
        ISubscriber<Step>,
        IDisposable
    {
        private readonly ILockstepEventPublishingService _publisher;
        private readonly IStepService _steps;
        private readonly ISimulationService _simulations;
        private readonly ILogger _logger;

        public State State { get; }

        public LockstepSimulation(
            State state,
            NetScope netScope,
            ISimulationService simulations, 
            IStepService steps, 
            IFiltered<ILockstepEventPublishingService> publisher, 
            IParallelService simulatedEntities,
            IGlobalSimulationService globalSimulationService,
            ILogger logger) : base(SimulationType.Lockstep, netScope, simulatedEntities, globalSimulationService)
        {
            _simulations = simulations;
            _steps = steps;
            _publisher = publisher.Instance ?? throw new ArgumentNullException();
            _logger = logger;

            this.State = state;
        }

        public override void Initialize(IServiceProvider provider)
        {
            base.Initialize(provider);

            _publisher.Initialize(base.Input);
        }

        public void Dispose()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            _steps.Update(gameTime);
        }

        public override void Input(ISimulationInputData data, Confidence confidence)
        {
            _publisher.Publish(data, confidence);
        }

        public void Process(in Tick message)
        {
            foreach (ISimulationInputData data in message.Data)
            {
                // At this point in time the data has successfully
                // been converted into lockstep server data,
                // publish it so.
                _simulations.Input(data, Confidence.Certain);
            }
        }

        public void Process(in Step message)
        {
            this.UpdateSystems(message);
        }
    }
}
