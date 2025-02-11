using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Ships.Systems
{
    public sealed class TractorBeamEmitterInputSystem(
        ITractorBeamEmitterService tractorBeamEmitterService
    ) : ISceneSystem,
        IEventSystem<Input_TractorBeamEmitter_Select>,
        IEventSystem<Input_TractorBeamEmitter_Deselect>
    {
        private readonly ITractorBeamEmitterService _tractorBeamEmitterService = tractorBeamEmitterService;

        [SequenceGroup<EventSequenceGroupEnum>(EventSequenceGroupEnum.Process)]
        public void Process(in VhId eventId, Input_TractorBeamEmitter_Select data)
        {
            this._tractorBeamEmitterService.Select(eventId, data.TractorBeamEmitterGlobalId, data.TargetNodeGlobalId);
        }

        [SequenceGroup<EventSequenceGroupEnum>(EventSequenceGroupEnum.Process)]
        public void Process(in VhId eventId, Input_TractorBeamEmitter_Deselect data)
        {
            this._tractorBeamEmitterService.Deselect(eventId, data.TractorBeamEmitterGlobalId, data.AttachToNodeSocketGlobalId);
        }
    }
}