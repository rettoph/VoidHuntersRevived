using Guppy.Core.Common;
using Guppy.Core.Logging.Common;
using Guppy.Core.Resources.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    public sealed class LockstepStrategy_Client(
        ISettingService settings,
        IGuppyScope scope,
        IStepEventService eventService,
        ILogger logger
    ) : LockstepStrategy(settings, scope, eventService, logger)
    {
    }
}