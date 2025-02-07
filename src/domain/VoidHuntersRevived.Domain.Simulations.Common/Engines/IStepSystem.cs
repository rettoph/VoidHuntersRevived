using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public interface IStepSystem : ISceneSystem
    {
        [RequireSequenceGroup<StepSequenceGroupEnum>]
        void Step(Step step);
    }
}