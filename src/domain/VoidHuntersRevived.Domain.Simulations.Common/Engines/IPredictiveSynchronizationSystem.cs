using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public interface IPredictiveSynchronizationSystem : IEngine
    {
        void Initialize(ILockstepStrategy lockstep);

        [RequireSequenceGroup<OnStepSequenceGroupEnum>]
        void Synchronize(Step step);
    }
}