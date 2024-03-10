﻿using Autofac;
using Guppy.Attributes;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;
using VoidHuntersRevived.Domain.Common.Constants;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [GuppyFilter<IVoidHuntersGameGuppy>()]
    internal abstract class LockstepSimulation : Simulation, ILockstepSimulation
    {
        private readonly IStepGroupEngine<Tick> _tickStepEnginesGroup;
        private readonly List<Tick> _history;

        internal int stepsPerTick;
        internal int stepsSinceTick;
        internal TimeSpan timeSinceStep;
        internal TimeSpan stepTimeSpan;
        internal Fix64 stepInterval;
        internal Step step;


        public Tick CurrentTick { get; private set; }

        public IEnumerable<Tick> History => _history;

        public event OnEventDelegate<EventDto>? OnEvent;

        public LockstepSimulation(ISettingProvider settings, ILifetimeScope scope) : base(SimulationType.Lockstep, scope)
        {
            _tickStepEnginesGroup = this.Engines.All().CreateStepEnginesGroup<Tick>();
            _history = new List<Tick>();

            this.stepsPerTick = settings.Get(Settings.StepsPerTick).Value;
            this.stepInterval = settings.Get(Settings.StepInterval).Value;
            this.stepsSinceTick = 0;
            this.timeSinceStep = TimeSpan.Zero;
            this.stepTimeSpan = TimeSpan.FromSeconds((double)stepInterval);
            this.step = new Step()
            {
                ElapsedTime = this.stepInterval,
                TotalTime = this.stepInterval
            };

            this.CurrentTick = Tick.First(Array.Empty<EventDto>());
        }

        public override void Update(GameTime realTime)
        {
            this.timeSinceStep += realTime.ElapsedGameTime;

            base.Update(realTime);

            if (this.TryGetNextTick(this.CurrentTick, out Tick? next))
            {
                this.DoTick(next);
            }
        }

        protected override void DoStep(Step step)
        {
            base.DoStep(step);

            this.stepsSinceTick++;

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
            this.stepsSinceTick = 0;

            if (tick.Events.Length == 0)
            {
                return;
            }

            foreach (EventDto @event in tick.Events)
            {
                this.Publish(@event);
            }

            this.OnEvent?.Invoke(new EventDto()
            {
                SourceId = NameSpace<LockstepSimulation>.Instance,
                Data = new EndOfTick()
                {
                    TickId = tick.Id
                }
            });

            _history.Add(tick);
        }

        public override void Publish(EventDto @event)
        {
            this.OnEvent?.Invoke(@event);

            base.Publish(@event);
        }
    }
}
