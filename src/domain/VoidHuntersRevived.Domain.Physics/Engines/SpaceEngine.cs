using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    [AutoLoad]
    internal sealed class SpaceEngine(ISpace space) : StrategyEngine, IOnStepEngine
    {
        private readonly ISpace _space = space;

        [SequenceGroup<OnStepSequenceGroup>(OnStepSequenceGroup.StepSpace)]
        public void OnStep(Step step)
        {
            _space.Step(step);
        }
    }
}
