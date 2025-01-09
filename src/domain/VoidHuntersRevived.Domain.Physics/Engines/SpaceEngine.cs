using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    public sealed class SpaceEngine(ISpace space) : StrategyEngine, IOnStepEngine
    {
        private readonly ISpace _space = space;

        [SequenceGroup<OnStepSequenceGroupEnum>(OnStepSequenceGroupEnum.StepSpace)]
        public void OnStep(Step step)
        {
            this._space.Step(step);
        }
    }
}