using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    [AutoLoad]
    [Sequence<EngineSequence>(EngineSequence.Group03)]
    [Sequence<StepSequence>(StepSequence.Cleanup)]
    internal class BodyEnabledEngine : StrategyEngine
    {
    }
}
