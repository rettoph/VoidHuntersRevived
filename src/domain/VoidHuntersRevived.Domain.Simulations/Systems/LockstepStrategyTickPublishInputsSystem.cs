using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    public class LockstepStrategyTickPublishInputsSystem(
        IStepEventService eventService
    ) : ISceneSystem,
        ITickSystem
    {
        private readonly IStepEventService _eventService = eventService;

        [SequenceGroup<TickSequenceGroupEnum>(TickSequenceGroupEnum.PublishInputs)]
        public void Tick(Tick tick)
        {
            if (tick.Inputs.Length == 0)
            {
                return;
            }

            foreach (EnqueuedStepInput @event in tick.Inputs)
            {
                this._eventService.Publish(@event.Id, @event.Data);
            }
        }
    }
}
