using Guppy.Attributes;
using Guppy.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    [AutoLoad]
    [Sequence<StepSequence>(StepSequence.ResourceManagerUpdate)]
    internal sealed class SpaceEngine : BasicEngine,
        IStepEngine<Step>
    {
        private readonly ISpace _space;

        public SpaceEngine(ISpace space)
        {
            _space = space;
        }

        public string name { get; } = nameof(SpaceEngine);

        public void Step(in Step _param)
        {
            _space.Step(_param);
        }
    }
}
