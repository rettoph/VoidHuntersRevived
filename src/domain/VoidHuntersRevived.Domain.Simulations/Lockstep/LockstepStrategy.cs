using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Providers;
using Guppy.Core.Resources.Common.Services;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Graphics.Common.Constants;
using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [SetSceneConfiguration<bool>(GraphicsSceneConfigurationKeys.SceneHasGraphicsEnabled, false)]
    public abstract class LockstepStrategy : Strategy, ILockstepStrategy
    {
        private readonly ActionSequenceGroup<OnTickSequenceGroup, Tick> _tickActions;
        private readonly List<Tick> _history;

        private TimeSpan _timeSinceStep;
        private int _stepsSinceTick;
        private readonly Step _step;

        public int StepsPerTick { get; }
        public Fix64 StepInterval { get; }
        public TimeSpan StepTimeSpan { get; }
        public TimeSpan TimeSinceStep
        {
            get => _timeSinceStep;
            protected set => _timeSinceStep = value;
        }
        public int StepsSinceTick
        {
            get => _stepsSinceTick;
            protected set => _stepsSinceTick = value;
        }


        public Tick CurrentTick { get; private set; }

        public IEnumerable<Tick> History => _history;

        public event OnEventDelegate<EventDto>? OnEvent;

        internal LockstepStrategy(
            ISettingService settings,
            Lazy<IEngineService> engineService,
            Lazy<ILoggerService> loggerService) : base(StrategyTypeEnum.Lockstep, engineService, loggerService)
        {
            _tickActions = new ActionSequenceGroup<OnTickSequenceGroup, Tick>(false);
            _history = [];
            _stepsSinceTick = 0;
            _timeSinceStep = TimeSpan.Zero;
            _step = new Step()
            {
                ElapsedTime = settings.GetValue(Settings.StepInterval),
                TotalTime = settings.GetValue(Settings.StepInterval)
            };

            this.StepsPerTick = settings.GetValue(Settings.StepsPerTick);
            this.StepInterval = settings.GetValue(Settings.StepInterval);
            this.StepTimeSpan = TimeSpan.FromSeconds((double)this.StepInterval);

            this.CurrentTick = Tick.First(Array.Empty<EventDto>());
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);

            _tickActions.Add([this.Tick_PublishEvents]);
            _tickActions.Add(this.Engines);
        }

        public override void Update(GameTime realTime)
        {
            _timeSinceStep += realTime.ElapsedGameTime;

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

            _step.TotalTime += _step.ElapsedTime;
            step = _step;
            return true;
        }

        protected override void DoStep(Step step)
        {
            base.DoStep(step);

            _stepsSinceTick++;

            if (this.TryGetNextTick(this.CurrentTick, out Tick? next))
            {
                this.DoTick(next);
            }
        }

        protected abstract bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next);
        protected virtual void DoTick(Tick tick)
        {
            this.CurrentTick = tick;

            _tickActions.Invoke(tick);

            _stepsSinceTick = 0;
            _history.Add(tick);
        }

        [SequenceGroup<OnTickSequenceGroup>(OnTickSequenceGroup.PublishEvents)]
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
