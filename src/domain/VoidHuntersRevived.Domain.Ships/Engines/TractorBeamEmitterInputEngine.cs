using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Ships.Engines
{
    public sealed class TractorBeamEmitterInputEngine(
        ITractorBeamEmitterService tractorBeamEmitterService,
        IEntityQueryService entityQueryService,
        ILogger logger) : StrategyEngine,
        IEventEngine<Input_TractorBeamEmitter_Select>,
        IEventEngine<Input_TractorBeamEmitter_Deselect>
    {
        private readonly ITractorBeamEmitterService _tractorBeamEmitterService = tractorBeamEmitterService;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ILogger _logger = logger;

        public void Process(VhId eventId, Input_TractorBeamEmitter_Select data)
        {
            _tractorBeamEmitterService.Select(eventId, data.TractorBeamEmitterGlobalId, data.TargetNodeGlobalId);
        }

        public void Process(VhId eventId, Input_TractorBeamEmitter_Deselect data)
        {
            _tractorBeamEmitterService.Deselect(eventId, data.TractorBeamEmitterGlobalId, data.AttachToSocketVhId);
        }
    }
}
