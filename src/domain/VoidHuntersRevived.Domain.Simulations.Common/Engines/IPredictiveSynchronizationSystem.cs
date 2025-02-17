using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public interface IPredictiveSynchronizationSystem : ISceneSystem
    {
        void Initialize(ILockstepStrategy lockstep);

        [RequireSequenceGroup<StepSequenceGroupEnum>]
        void Synchronize(Step step);
    }
}