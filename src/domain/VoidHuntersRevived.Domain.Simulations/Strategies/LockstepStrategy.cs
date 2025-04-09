using Guppy.Core.Common;
using Guppy.Core.Logging.Common;
using Guppy.Core.Assets.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;

namespace VoidHuntersRevived.Domain.Simulations.Strategies
{
    public class LockstepStrategy : Strategy, ILockstepStrategy
    {
        public LockstepStrategy(
            ISettingService settings,
            IGuppyScope scope,
            IStepEventService eventService,
            ILogger logger) : base(StrategyTypeEnum.Lockstep, scope, eventService, logger)
        {
        }
    }
}