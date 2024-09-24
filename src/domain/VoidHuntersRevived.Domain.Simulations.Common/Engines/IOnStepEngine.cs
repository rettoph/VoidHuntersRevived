using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    public interface IOnStepEngine
    {
        [RequireSequenceGroup<OnStepSequenceGroup>]
        void OnStep(Step step);
    }
}
