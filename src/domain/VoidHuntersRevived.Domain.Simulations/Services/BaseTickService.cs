using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Resources.Common.Services;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public abstract class BaseTickService(
        ISettingService settingService
    ) : ITickService
    {
        public int StepsPerTick { get; } = settingService.GetValue(Settings.StepsPerTick);
        public int NextTickId { get; private set; }

        public virtual void Reset()
        {
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
                this.NextTickId++;
                return true;
            }

            return false;
        }

        protected abstract bool TryDequeue([MaybeNullWhen(false)] out Tick tick);

        public abstract EnqueueTickResponseEnum TryEnqueue(Tick tick);
    }
}
