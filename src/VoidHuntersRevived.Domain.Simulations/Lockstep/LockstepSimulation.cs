﻿using Guppy.Attributes;
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
using Guppy.Common.Providers;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Entities;
using Autofac;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Domain.Entities.Extensions;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [GuppyFilter<IVoidHuntersGameGuppy>()]
    internal abstract class LockstepSimulation : Simulation, ILockstepSimulation
    {
        private readonly IStepGroupEngine<Tick> _tickStepEnginesGroup;
        private readonly List<Tick> _history;

        public Tick CurrentTick { get; private set; }

        public IEnumerable<Tick> History => _history;

        public event OnEventDelegate<EventDto>? OnEvent;

        public LockstepSimulation(ILifetimeScope scope) : base(SimulationType.Lockstep, scope)

        {
            _tickStepEnginesGroup = this.Engines.All().CreateStepEnginesGroup<Tick>();
            _history = new List<Tick>();

            this.CurrentTick = Tick.First();
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

            _tickStepEnginesGroup.Step(tick);

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

        protected override void Publish(EventDto @event)
        {
            this.OnEvent?.Invoke(@event);

            base.Publish(@event);
        }
    }
}
