using Guppy.Attributes;
using Guppy.Common.Attributes;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    [AutoLoad]
    [Sequence<StepSequence>(StepSequence.Cleanup)]
    internal class BodyEnabledEngine : BasicEngine
    {
    }
}
