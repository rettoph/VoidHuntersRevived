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
        private readonly List<Tick> _history;

        public Tick CurrentTick { get; private set; }

        public IEnumerable<Tick> History => _history;

        public event OnEventDelegate<EventDto>? OnEvent;

        public LockstepSimulation(
            ISpaceFactory spaceFactory,
            IFilteredProvider filtered,
            IBus bus) : base(SimulationType.Lockstep, spaceFactory, filtered, bus)

        {
            _tickEngines = new TickEngineGroup(this.World.Engines.OfType<ITickEngine>());
            _history = new List<Tick>();

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

            if (tick.Events.Length == 0)
            {
                return;
            }

            foreach (EventDto @event in tick.Events)
            {
                this.Publish(@event);
            }

            _history.Add(tick);
        }

        public override void Publish(EventDto data)
        {
            this.publisher.Publish(data);
            this.OnEvent?.Invoke(data);
        }
    }
}
