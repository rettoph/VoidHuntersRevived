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
    internal abstract class LockstepSimulation : Simulation<Common.Simulations.Components.Lockstep>, ISimulation,
        ILockstepSimulation,
        ISubscriber<Step>,
        ISubscriber<Tick>,
        IDisposable
    {
        private readonly IStepService _steps;
        private readonly ISimulationEventPublishingService _input;

        public State State { get; }

        public LockstepSimulation(
            State state,
            ISimulationEventPublishingService input,
            IStepService steps, 
            IParallelableService parallelables,
            IGlobalSimulationService globalSimulationService) : base(SimulationType.Lockstep, parallelables, globalSimulationService)
        {
            _steps = steps;
            _input = input;

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

        public override void Publish(ISimulationEventData data)
        {
            _input.Publish(this, data);
        }

        public void Process(in Step message)
        {
            this.UpdateSystems(message);
        }

        public void Process(in Tick message)
        {
            foreach (SimulationInput input in message.Inputs)
            {
                this.Publish(input);
            }
        }
    }
}
