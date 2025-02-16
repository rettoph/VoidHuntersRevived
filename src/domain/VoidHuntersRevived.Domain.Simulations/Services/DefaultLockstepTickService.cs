using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Resources.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public class DefaultLockstepTickService(DefaultLockstepStepEventService eventService, ISettingService settingService) : BaseLockstepTickService(settingService)
    {
        private readonly DefaultLockstepStepEventService _eventService = eventService;

        public override bool ShouldStep(bool timeSinceLastStepLessThanStepInterval)
        {
            return timeSinceLastStepLessThanStepInterval == false;
        }

        public override EnqueueTickResponseEnum TryEnqueue(Tick tick)
        {
            throw new NotImplementedException();
        }

        protected override bool TryDequeue([MaybeNullWhen(false)] out Tick tick)
        {
            tick = Tick.Create(this.NextTickId, this._eventService.FlushInputs());
            return true;
        }
    }
}
