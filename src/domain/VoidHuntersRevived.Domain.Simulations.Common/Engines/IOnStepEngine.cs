using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    public interface IOnStepEngine : IEngine
    {
        [RequireSequenceGroup<OnStepSequenceGroup>]
        void OnStep(Step step);
    }
}
