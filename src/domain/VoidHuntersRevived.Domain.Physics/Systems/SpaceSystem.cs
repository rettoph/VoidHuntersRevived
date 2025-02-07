using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Physics.Systems
{
    public sealed class SpaceSystem(ISpace space) : ISceneSystem, IStepSystem
    {
        private readonly ISpace _space = space;

        [SequenceGroup<StepSequenceGroupEnum>(StepSequenceGroupEnum.StepSpace)]
        public void Step(Step step)
        {
            this._space.Step(step);
        }
    }
}