using Guppy.Attributes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Ships.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Ships.Events;

namespace VoidHuntersRevived.Game.Ships.Engines
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
            if (!_entities.TryGetId(data.ShipVhId, out EntityId tractorBeamEmitterId) || _entities.GetState(tractorBeamEmitterId) != EntityState.Spawned)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - ShipVhId {ShipId} not found.", nameof(TractorBeamEmitterInputEngine), nameof(Process), nameof(Input_TractorBeamEmitter_Select), data.ShipVhId.Value);
                return;
            }

            if (!_entities.TryGetId(data.TargetVhId, out EntityId targetNodeId) || _entities.GetState(targetNodeId) != EntityState.Spawned)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TargetVhId {TargetId} not found.", nameof(TractorBeamEmitterInputEngine), nameof(Process), nameof(Input_TractorBeamEmitter_Select), data.TargetVhId.Value);
                return;
            }

            _tractorBeamEmitters.Select(tractorBeamEmitterId, targetNodeId);
        }

        public void Process(VhId eventId, Input_TractorBeamEmitter_Deselect data)
        {
            if (!_entities.TryGetId(data.ShipVhId, out EntityId tractorBeamEmitterId) || _entities.GetState(tractorBeamEmitterId) != EntityState.Spawned)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - ShipVhId {ShipId} not found.", nameof(TractorBeamEmitterInputEngine), nameof(Process), nameof(Input_TractorBeamEmitter_Deselect), data.ShipVhId.Value);
                return;
            }

            _tractorBeamEmitters.Deselect(tractorBeamEmitterId);
        }
    }
}
