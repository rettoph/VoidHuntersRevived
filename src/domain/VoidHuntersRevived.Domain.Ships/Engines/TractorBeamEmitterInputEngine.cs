using Guppy.Attributes;
using Serilog;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Ships.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Ships.Events;

namespace VoidHuntersRevived.Domain.Ships.Engines
{
    [AutoLoad]
    internal sealed class TractorBeamEmitterInputEngine : BasicEngine,
        IEventEngine<Input_TractorBeamEmitter_Select>,
        IEventEngine<Input_TractorBeamEmitter_Deselect>
    {
        private readonly ITractorBeamEmitterService _tractorBeamEmitters;
        private readonly IEntityService _entities;
        private readonly ILogger _logger;

        public TractorBeamEmitterInputEngine(ITractorBeamEmitterService tractorBeamEmitters, IEntityService entities, ILogger logger)
        {
            _tractorBeamEmitters = tractorBeamEmitters;
            _entities = entities;
            _logger = logger;
        }

        public void Process(VhId eventId, Input_TractorBeamEmitter_Select data)
        {
            if (!_entities.TryGetId(data.ShipVhId, out EntityId tractorBeamEmitterId))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - ShipVhId {ShipId} not found.", nameof(TractorBeamEmitterInputEngine), nameof(Process), nameof(Input_TractorBeamEmitter_Select), data.ShipVhId.Value);
                return;
            }

            if (!_entities.TryGetId(data.TargetVhId, out EntityId targetNodeId))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TargetVhId {TargetId} not found.", nameof(TractorBeamEmitterInputEngine), nameof(Process), nameof(Input_TractorBeamEmitter_Select), data.TargetVhId.Value);
                return;
            }

            _tractorBeamEmitters.Select(eventId, tractorBeamEmitterId, targetNodeId);
        }

        public void Process(VhId eventId, Input_TractorBeamEmitter_Deselect data)
        {
            if (!_entities.TryGetId(data.ShipVhId, out EntityId tractorBeamEmitterId))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - ShipVhId {ShipId} not found.", nameof(TractorBeamEmitterInputEngine), nameof(Process), nameof(Input_TractorBeamEmitter_Deselect), data.ShipVhId.Value);
                return;
            }

            _tractorBeamEmitters.Deselect(eventId, tractorBeamEmitterId, data.AttachToSocketVhId);
        }
    }
}
