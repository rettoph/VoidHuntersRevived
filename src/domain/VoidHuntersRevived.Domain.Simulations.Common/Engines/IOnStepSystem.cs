using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public interface IOnStepSystem : IEngine
    {
        [RequireSequenceGroup<OnStepSequenceGroupEnum>]
        void OnStep(Step step);
    }
}