using Autofac;
using Guppy.Game.Common.Attributes;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [SceneFilter<IVoidHuntersGameScene>()]
    internal abstract class LockstepStrategy : Strategy, ILockstepSimulation
    {
        private IStepGroupEngine<Tick> _tickStepEnginesGroup;
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

        public LockstepStrategy(ILifetimeScope scope) : base(StrategyTypeEnum.Lockstep, scope)
        {
            _history = new List<Tick>();
            _tickStepEnginesGroup = null!;

            this.stepsPerTick = Settings.StepsPerTick.Value;
            this.stepInterval = Settings.StepInterval.Value;
            this.stepsSinceTick = 0;
            this.timeSinceStep = TimeSpan.Zero;
            this.stepTimeSpan = TimeSpan.FromSeconds((double)this.stepInterval);
            this.step = new Step()
            {
                ElapsedTime = this.stepInterval,
                TotalTime = this.stepInterval
            };

            this.CurrentTick = Tick.First(Array.Empty<EventDto>());
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);

            _tickStepEnginesGroup = this.engines.All().CreateStepEnginesGroup<Tick>();
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
                SourceId = NameSpace<LockstepStrategy>.Instance,
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
