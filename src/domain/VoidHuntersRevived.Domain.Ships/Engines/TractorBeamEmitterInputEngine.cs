using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Ships.Engines
{
    public sealed class TractorBeamEmitterInputEngine(
        ITractorBeamEmitterService tractorBeamEmitterService
    ) : StrategyEngine,
        IEventEngine<Input_TractorBeamEmitter_Select>,
        IEventEngine<Input_TractorBeamEmitter_Deselect>
    {
        private readonly ITractorBeamEmitterService _tractorBeamEmitterService = tractorBeamEmitterService;

        public void Process(VhId eventId, Input_TractorBeamEmitter_Select data) => this._tractorBeamEmitterService.Select(eventId, data.TractorBeamEmitterGlobalId, data.TargetNodeGlobalId);

        public void Process(VhId eventId, Input_TractorBeamEmitter_Deselect data) => this._tractorBeamEmitterService.Deselect(eventId, data.TractorBeamEmitterGlobalId, data.AttachToNodeSocketGlobalId);
    }
}