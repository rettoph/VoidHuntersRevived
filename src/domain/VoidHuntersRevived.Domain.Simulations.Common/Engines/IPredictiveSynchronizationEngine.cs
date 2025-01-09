using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    public interface IPredictiveSynchronizationEngine : IEngine
    {
        void Initialize(ILockstepStrategy lockstep);

        [RequireSequenceGroup<OnStepSequenceGroupEnum>]
        void Synchronize(Step step);
    }
}