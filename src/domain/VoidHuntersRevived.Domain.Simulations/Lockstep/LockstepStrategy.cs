using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common.Services;
using Guppy.Core.Resources.Common.Services;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    public abstract class LockstepStrategy : Strategy, ILockstepStrategy
    {
        private readonly ActionSequenceGroup<OnTickSequenceGroupEnum, Tick> _tickActions;
        private readonly List<Tick> _history;
        private readonly Step _step;

        public int StepsPerTick { get; }
        public Fix64 StepInterval { get; }
        public TimeSpan StepTimeSpan { get; }
        public TimeSpan TimeSinceStep { get; protected set; }
        public int StepsSinceTick { get; protected set; }


        public Tick CurrentTick { get; private set; }

        public IEnumerable<Tick> History => this._history;

        public event OnEventDelegate<EventDto>? OnEvent;

        internal LockstepStrategy(
            ISettingService settings,
            Lazy<IEngineService> engineService,
            Lazy<ILoggerService> loggerService) : base(StrategyTypeEnum.Lockstep, engineService, loggerService)
        {
            this._tickActions = new ActionSequenceGroup<OnTickSequenceGroupEnum, Tick>(false);
            this._history = [];
            this.StepsSinceTick = 0;
            this.TimeSinceStep = TimeSpan.Zero;
            this._step = new Step()
            {
                ElapsedTime = settings.GetValue(Settings.StepInterval),
                TotalTime = settings.GetValue(Settings.StepInterval)
            };

            this.StepsPerTick = settings.GetValue(Settings.StepsPerTick);
            this.StepInterval = settings.GetValue(Settings.StepInterval);
            this.StepTimeSpan = TimeSpan.FromSeconds((double)this.StepInterval);

            this.CurrentTick = Tick.First([]);
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);

            this._tickActions.Add([this.Tick_PublishEvents]);
            this._tickActions.Add(this.Engines);
        }

        public override void Update(GameTime realTime)
        {
            this.TimeSinceStep += realTime.ElapsedGameTime;

            base.Update(realTime);

            if (this.TryGetNextTick(this.CurrentTick, out Tick? next))
            {
                this.DoTick(next);
            }
        }

        protected abstract bool ShouldStep(GameTime realTime);

        protected override bool TryGetNextStep(GameTime realTime, [MaybeNullWhen(false)] out Step step)
        {
            if (this.ShouldStep(realTime) == false)
            {
                step = null;
                return false;
            }

            this._step.TotalTime += this._step.ElapsedTime;
            step = this._step;
            return true;
        }

        protected override void DoStep(Step step)
        {
            base.DoStep(step);

            this.StepsSinceTick++;

            if (this.TryGetNextTick(this.CurrentTick, out Tick? next))
            {
                this.DoTick(next);
            }
        }

        protected abstract bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next);
        protected virtual void DoTick(Tick tick)
        {
            this.CurrentTick = tick;

            this._tickActions.Invoke(tick);

            this.StepsSinceTick = 0;
            this._history.Add(tick);
        }

        [SequenceGroup<OnTickSequenceGroupEnum>(OnTickSequenceGroupEnum.PublishEvents)]
        private void Tick_PublishEvents(Tick tick)
        {
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
        }

        public override void Publish(EventDto @event)
        {
            this.OnEvent?.Invoke(@event);

            base.Publish(@event);
        }
    }
}