using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Physics.Systems
{
    public sealed class SpaceSystem(ISpace space) : StrategySystem, IOnStepSystem
    {
        private readonly ISpace _space = space;

        [SequenceGroup<OnStepSequenceGroupEnum>(OnStepSequenceGroupEnum.StepSpace)]
        public void OnStep(Step step)
        {
            this._space.Step(step);
        }
    }
}