using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common;
using Guppy.Core.Resources.Common.Services;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    public abstract class LockstepStrategy : Strategy, ILockstepStrategy
    {
        public LockstepStrategy(
            ISettingService settings,
            IGuppyScope scope,
            IStepEventService eventService,
            ILogger logger) : base(StrategyTypeEnum.Lockstep, scope, eventService, logger)
        {
        }

        [SequenceGroup<TickSequenceGroupEnum>(TickSequenceGroupEnum.PublishEvents)]
        public void Tick_PublishEvents(Tick tick)
        {
            if (tick.Inputs.Length == 0)
            {
                return;
            }

            foreach (EnqueuedStepInput @event in tick.Inputs)
            {
                this.Events.Publish(@event.Id, @event.Data);
            }

            EndOfTick endOfTick = new()
            {
                TickId = tick.Id
            };
            Id<IStepEvent> endOfTickId = new(endOfTick.CalculateHash(NameSpace<LockstepStrategy>.Instance));
        }
    }
}