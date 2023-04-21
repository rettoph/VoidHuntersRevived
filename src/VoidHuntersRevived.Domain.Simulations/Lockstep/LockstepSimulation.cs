using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using Guppy.Network.Identity;
using LiteNetLib;
using Microsoft.Xna.Framework;
using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Providers;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Services;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [GuppyFilter<IGameGuppy>()]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class LockstepSimulation : Simulation<Common.Simulations.Components.Lockstep>, ISimulation,
        ILockstepSimulation,
        ISubscriber<Tick>,
        ISubscriber<Step>,
        IDisposable
    {
        private readonly ILockstepInputService _publisher;
        private readonly IStepService _steps;
        private readonly ISimulationService _simulations;
        private readonly ILogger _logger;

        public State State { get; }

        public LockstepSimulation(
            State state,
            ISimulationService simulations, 
            IStepService steps, 
            IFiltered<ILockstepInputService> publisher, 
            IParallelableService simulatedEntities,
            IGlobalSimulationService globalSimulationService,
            ILogger logger) : base(SimulationType.Lockstep, simulatedEntities, globalSimulationService)
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
        }

        protected override void Update(GameTime gameTime)
        {
            _steps.Update(gameTime);
        }

        public void Process(in Tick message)
        {
            foreach (UserInput input in message.Inputs)
            {
                if(Simulation.Input.Factory.TryCreate(SimulationType.Lockstep, input.User, input.Data, this, out IEvent? instance))
                {
                    this.PublishEvent(instance);
                }
            }
        }

        public void Process(in Step message)
        {
            this.UpdateSystems(message);
        }

        public override void Input(ParallelKey user, IData data)
        {
            _publisher.Input(user, data);
        }
    }
}
