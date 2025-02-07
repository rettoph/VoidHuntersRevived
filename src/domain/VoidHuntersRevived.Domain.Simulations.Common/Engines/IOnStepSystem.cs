using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public interface IOnStepSystem : ISceneSystem
    {
        [RequireSequenceGroup<OnStepSequenceGroupEnum>]
        void OnStep(Step step);
    }
}