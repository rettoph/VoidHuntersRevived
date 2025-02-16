using Guppy.Core.Messaging.Common;
using Guppy.Core.Resources.Common.Services;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public class DefaultLockstepStepService(
        ISettingService settingService,
        ITickService tickService,
        IMessageBus messageBus
    ) : IStepService
    {
        private readonly ITickService _tickService = tickService;
        private readonly IMessageBus _messageBus = messageBus;
        private readonly Step _step = new()
        {
            ElapsedTime = settingService.GetValue(Settings.StepInterval),
            TotalTime = Fix64.Zero
        };

        public TimeSpan TimeSinceStep { get; private set; }
        public int StepsSinceTick { get; private set; }
        public int StepsPerTick { get; } = settingService.GetValue(Settings.StepsPerTick);
        public TimeSpan StepTimeSpan { get; } = TimeSpan.FromSeconds((double)settingService.GetValue(Settings.StepInterval).Value);

        public bool ShouldStep()
        {
            if (this.StepsSinceTick > this.StepsPerTick)
            {
                throw new Exception();
            }

            if (this.StepsSinceTick == this.StepsPerTick)
            {
                return false;
            }

            return this._tickService.ShouldStep(this.TimeSinceStep < this.StepTimeSpan);
        }

        public void Update(GameTime gameTime)
        {
            this.TimeSinceStep += gameTime.ElapsedGameTime;

            while (this.ShouldStep() == true)
            {
                this.DoStep();
            }

            if (this._tickService.TryDequeue(this.StepsSinceTick, out Tick? next))
            {
                this.DoTick(next);
            }
        }

        protected virtual void DoStep()
        {
            this.StepsSinceTick++;
            this.TimeSinceStep -= this.StepTimeSpan;
            this._step.TotalTime += this._step.ElapsedTime;
            this._messageBus.Publish<StepSequenceGroupEnum, Step>(this._step);

            if (this._tickService.TryDequeue(this.StepsSinceTick, out Tick? next))
            {
                this.DoTick(next);
            }
        }

        protected virtual void DoTick(Tick tick)
        {
            this.StepsSinceTick = 0;
            this._messageBus.Publish<TickSequenceGroupEnum, Tick>(tick);
        }
    }
}
