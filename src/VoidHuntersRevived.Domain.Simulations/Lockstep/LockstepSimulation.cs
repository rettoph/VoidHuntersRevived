using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using Guppy.Network.Identity;
using Guppy.Resources.Providers;
using LiteNetLib;
using Microsoft.Xna.Framework;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Physics.Factories;
using Guppy.Common.Providers;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Simulations.EnginesGroups;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [GuppyFilter<IGameGuppy>()]
    internal abstract class LockstepSimulation : Simulation, ILockstepSimulation
    {
        private readonly TickEngineGroup _tickEngines;

        public Tick CurrentTick { get; private set; }

        public LockstepSimulation(
            ISpaceFactory spaceFactory,
            IFilteredProvider filtered,
            IBus bus) : base(SimulationType.Lockstep, spaceFactory, filtered, bus)

        {
            _tickEngines = new TickEngineGroup(this.World.Engines.OfType<ITickEngine>());

            this.CurrentTick = Tick.First();
        }

        public override void Initialize(ISimulationService simulations)
        {
            base.Initialize(simulations);

            foreach (ISimulationEngine<ILockstepSimulation> system in this.World.Engines.OfType<ISimulationEngine<ILockstepSimulation>>())
            {
                system.Initialize(this);
            }
        }

        public override void Update(GameTime realTime)
        {
            base.Update(realTime);

            if (this.TryGetNextTick(this.CurrentTick, out Tick? next))
            {
                this.DoTick(next);
            }
        }

        protected abstract bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next);
        protected virtual void DoTick(Tick tick)
        {
            this.CurrentTick = tick;

            _tickEngines.Step(tick);

            foreach (EventDto @event in tick.Events)
            {
                this.Publish(@event);
            }
        }

        public override void Publish(EventDto data)
        {
            this.publisher.Publish(data);
        }
    }
}
