using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    public class PredictiveStepEventCleanSystem(PredictiveStepEventService eventService) : ISceneSystem
    {
        public readonly PredictiveStepEventService _eventService = eventService;

        [SequenceGroup<StepSequenceGroupEnum>(StepSequenceGroupEnum.Begin)]
        public void BeginStep(Step step)
        {
            this._eventService.CleanConfirmedPredictions();
        }

        [SequenceGroup<StepSequenceGroupEnum>(StepSequenceGroupEnum.Begin)]
        public void EndStep(Step step)
        {
            this._eventService.CleanFailedPredictions(step);
        }
    }
}
