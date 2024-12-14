using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Components;
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
            if (!_entityQueryService.TryGetEntity<TractorBeamEmitter>(data.TractorBeamEmitterGlobalId, out var tractorBeamEmitter))
            {
                _logger.Warning("TractorBeamEmitterGlobalId {TractorBeamEmitterGlobalId} not found.", data.TractorBeamEmitterGlobalId.Value);
                return;
            }

            if (!_entityQueryService.TryGetEntity<Node>(data.TargetNodeGlobalId, out var targetNode))
            {
                _logger.Warning("TargetNodeGlobalId {TargetNodeGlobalId} not found.", data.TargetNodeGlobalId.Value);
                return;
            }

            _tractorBeamEmitterService.Select(eventId, tractorBeamEmitter, targetNode);
        }

        public void Process(VhId eventId, Input_TractorBeamEmitter_Deselect data)
        {
            if (!_entityQueryService.TryGetEntity<TractorBeamEmitter>(data.TractorBeamEmitterGlobalId, out var tractorBeamEmitter))
            {
                _logger.Warning("ShipVhId {ShipId} not found.", data.TractorBeamEmitterGlobalId.Value);
                return;
            }

            _tractorBeamEmitterService.Deselect(eventId, tractorBeamEmitter, data.AttachToSocketVhId);
        }
    }
}
