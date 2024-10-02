using Guppy.Core.Common.Attributes;
using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Ships.Engines
{
    [AutoLoad]
    internal sealed class TractorBeamEmitterInputEngine(
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
            if (!_entityQueryService.TryGetId(data.ShipVhId, out EntityId tractorBeamEmitterId))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - ShipVhId {ShipId} not found.", nameof(TractorBeamEmitterInputEngine), nameof(Process), nameof(Input_TractorBeamEmitter_Select), data.ShipVhId.Value);
                return;
            }

            if (!_entityQueryService.TryGetId(data.TargetVhId, out EntityId targetNodeId))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TargetVhId {TargetId} not found.", nameof(TractorBeamEmitterInputEngine), nameof(Process), nameof(Input_TractorBeamEmitter_Select), data.TargetVhId.Value);
                return;
            }

            _tractorBeamEmitterService.Select(eventId, tractorBeamEmitterId, targetNodeId);
        }

        public void Process(VhId eventId, Input_TractorBeamEmitter_Deselect data)
        {
            if (!_entityQueryService.TryGetId(data.ShipVhId, out EntityId tractorBeamEmitterId))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - ShipVhId {ShipId} not found.", nameof(TractorBeamEmitterInputEngine), nameof(Process), nameof(Input_TractorBeamEmitter_Deselect), data.ShipVhId.Value);
                return;
            }

            _tractorBeamEmitterService.Deselect(eventId, tractorBeamEmitterId, data.AttachToSocketVhId);
        }
    }
}
