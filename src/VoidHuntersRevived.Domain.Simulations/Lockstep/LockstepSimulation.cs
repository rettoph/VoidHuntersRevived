using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using Guppy.Network.Identity;
using LiteNetLib;
using Microsoft.Xna.Framework;
using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.ECS;
using VoidHuntersRevived.Common.ECS.Factories;
using VoidHuntersRevived.Common.Physics.Factories;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Utilities;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [GuppyFilter<IGameGuppy>()]
    internal class LockstepSimulation : Simulation, ISimulation,
        ILockstepSimulation,
        ISubscriber<Step>,
        ISubscriber<Tick>,
        IDisposable
    {
        public IGameState State { get; }

        public LockstepSimulation(
            IFiltered<IGameState> state,
            IWorldFactory worldFactory,
            ISpaceFactory spaceFactory,
            IBus bus,
            IBroker broker) : base(SimulationType.Lockstep, worldFactory, spaceFactory, bus)
        {
            this.State = state.Instance;
        }

        public override void Initialize(ISimulationService simulations)
        {
            base.Initialize(simulations);

            this.bus.Subscribe(this);
        }

        protected override void Update(GameTime gameTime)
        {
            this.State.Update(gameTime);
        }

        public override void Enqueue(EventDto input)
        {
            this.State.Enqueue(input);
        }

        public override void Publish(EventDto data)
        {
            this.publisher.Publish(data);
        }

        public void Process(in Step message)
        {
            this.World.Update(message);
        }

        public void Process(in Tick message)
        {
            foreach (EventDto data in message.Events)
            {
                this.Publish(data);
            }
        }
    }
}
