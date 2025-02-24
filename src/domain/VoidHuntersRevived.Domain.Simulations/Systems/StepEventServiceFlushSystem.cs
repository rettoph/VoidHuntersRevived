using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    public class StepEventServiceFlushSystem(IStepEventService stepEventService) : IStepSystem
    {
        private readonly IStepEventService _stepEventService = stepEventService;

        [SequenceGroup<StepSequenceGroupEnum>(StepSequenceGroupEnum.PublishEvents)]
        public void Step(Step step)
        {
            this._stepEventService.Flush();
        }
    }
}
