﻿using Guppy.Attributes;
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

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [GuppyFilter<IGameGuppy>()]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal class LockstepSimulation : Simulation<Common.Simulations.Components.Lockstep>, ISimulation,
        ILockstepSimulation,
        ISubscriber<Step>,
        ISubscriber<Tick>,
        IDisposable
    {
        private readonly ISimulationEventPublishingService _events;
        private readonly IBus _bus;

        public IState State { get; }

        public LockstepSimulation(
            IFiltered<IState> state,
            IBus bus,
            ISimulationEventPublishingService events,
            IParallelableService parallelables,
            IGlobalSimulationService globalSimulationService) : base(SimulationType.Lockstep, parallelables, globalSimulationService)
        {
            _events = events;
            _bus = bus;

            this.State = state.Instance;
        }

        public override void Initialize(IServiceProvider provider)
        {
            base.Initialize(provider);
        }

        protected override void Update(GameTime gameTime)
        {
            this.State.Update(gameTime);
        }

        public override void Enqueue(SimulationEventData input)
        {
            this.State.Enqueue(input);
        }


        public override ISimulationEvent Publish(SimulationEventData data)
        {
            return _events.Publish(this, data);
        }

        public void Process(in Step message)
        {
            this.UpdateSystems(message);
        }

        public void Process(in Tick message)
        {
            foreach (SimulationEventData data in message.Events)
            {
                this.Publish(data);
            }
        }
    }
}
