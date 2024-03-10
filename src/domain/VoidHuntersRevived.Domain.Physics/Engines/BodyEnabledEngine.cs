using Guppy.Attributes;
using Guppy.Common.Attributes;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    [AutoLoad]
    [Sequence<StepSequence>(StepSequence.Cleanup)]
    internal class BodyEnabledEngine : BasicEngine
    {
    }
}
