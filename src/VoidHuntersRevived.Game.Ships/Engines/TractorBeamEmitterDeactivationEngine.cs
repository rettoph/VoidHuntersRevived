using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Ships.Events;
using VoidHuntersRevived.Common.Entities.Services;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Ships.Components;

namespace VoidHuntersRevived.Game.Ships.Engines
{
    [AutoLoad]
    internal sealed class TractorBeamEmitterDeactivationEngine : BasicEngine,
        IEventEngine<TractorBeamEmitter_TryDeactivate>
    {
        private readonly IEntityService _entities;
        private readonly ISocketService _socketService;
        private readonly ILogger _logger;

        public TractorBeamEmitterDeactivationEngine(
            IEntityService entities,
            ISocketService socketService,
            ILogger logger)
        {
            _entities = entities;
            _socketService = socketService;
            _logger = logger;
        }

        public void Process(VhId eventId, TractorBeamEmitter_TryDeactivate data)
        {
            if(_entities.TryGetId(data.TargetVhId, out _) == false)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TargetVhId {TargetVhId} not found.", nameof(TractorBeamEmitterDeactivationEngine), nameof(Process), nameof(TractorBeamEmitter_TryDeactivate), data.TargetVhId);

                return;
            }

            EntityId tractorBeamEmitterId = _entities.GetId(data.ShipVhId);
            var tractorBeamEmitters = this.entitiesDB.QueryEntitiesAndIndex<TractorBeamEmitter>(tractorBeamEmitterId.EGID, out uint tractorBeamEmitterIndex);
            var (tacticals, _) = this.entitiesDB.QueryEntities<Tactical>(tractorBeamEmitterId.EGID.groupID);

            ref TractorBeamEmitter tractorBeamEmitter = ref tractorBeamEmitters[tractorBeamEmitterIndex];
            ref Tactical tactical = ref tacticals[tractorBeamEmitterIndex];

            if (tractorBeamEmitter.TargetId.VhId != data.TargetVhId)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TargetVhId has changed from {OldTargetId} to {NewTargetId}", nameof(TractorBeamEmitterDeactivationEngine), nameof(Process), nameof(TractorBeamEmitter_TryDeactivate), data.TargetVhId.Value, tractorBeamEmitter.TargetId.VhId.Value);

                return;
            }

            // Flush just in case the target was created in the same frame
            _entities.Flush();
            this.entitiesDB.QueryEntity<Tractorable>(tractorBeamEmitter.TargetId.EGID).IsTractored = false;


            // Ensure the emitter is deactivated
            tractorBeamEmitter.Active = false;
            tractorBeamEmitter.TargetId = default!;
            tactical.RemoveUse();
        }
    }
}
