using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Resources.Common.Services;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public abstract class BaseLockstepTickService(
        ISettingService settingService
    ) : ITickService
    {
        public int StepsPerTick { get; } = settingService.GetValue(Settings.StepsPerTick);
        public int LastTickId { get; private set; }
        public int NextTickId { get; private set; }

        public virtual void Reset()
        {
            this.LastTickId = -1;
            this.NextTickId = 0;
        }

        public bool TryDequeue(int stepsSinceTick, [MaybeNullWhen(false)] out Tick tick)
        {
            if (stepsSinceTick > this.StepsPerTick)
            {
                // This should not be possible.
                // How can we have more steps takens than allowed
                // per tick??
                throw new NotImplementedException();
            }

            if (stepsSinceTick < this.StepsPerTick)
            {
                tick = null;
                return false;
            }

            if (this.TryDequeue(out tick) == true)
            {
                this.LastTickId = this.NextTickId++;
                return true;
            }

            return false;
        }

        protected abstract bool TryDequeue([MaybeNullWhen(false)] out Tick tick);

        public abstract EnqueueTickResponseEnum TryEnqueue(Tick tick);

        public abstract bool ShouldStep(bool timeSinceLastStepLessThanStepInterval);
    }
}
