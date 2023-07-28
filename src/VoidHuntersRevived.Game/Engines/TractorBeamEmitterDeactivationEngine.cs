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
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Events;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Factories;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class TractorBeamEmitterDeactivationEngine : BasicEngine,
        IEventEngine<TractorBeamEmitter_TryDeactivate>,
        IEventEngine<TractorBeamEmitter_Deactivate>
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
            EntityId tractorBeamEmitterId = _entities.GetId(data.ShipVhId);
            ref TractorBeamEmitter tractorBeamEmitter = ref this.entitiesDB.QueryMappedEntities<TractorBeamEmitter>(tractorBeamEmitterId.EGID.groupID).Entity(tractorBeamEmitterId.EGID.entityID);

            if(_entities.TryGetId(tractorBeamEmitter.TargetId.VhId, out _) == false)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TargetVhId {OldTargetId} not found.", nameof(TractorBeamEmitterDeactivationEngine), nameof(Process), nameof(TractorBeamEmitter_TryDeactivate), tractorBeamEmitter.TargetId.VhId.Value);

                return;
            }

            this.Simulation.Publish(eventId, new TractorBeamEmitter_Deactivate()
            {
                TractorBeamEmitterVhId = data.ShipVhId,
                TargetVhId = tractorBeamEmitter.TargetId.VhId,
                AttachTo = data.AttachTo
            });
        }

        public void Process(VhId eventId, TractorBeamEmitter_Deactivate data)
        {
            EntityId tractorBeamEmitterId = _entities.GetId(data.TractorBeamEmitterVhId);
            var tractorBeamEmitters = this.entitiesDB.QueryEntitiesAndIndex<TractorBeamEmitter>(tractorBeamEmitterId.EGID, out uint tractorBeamEmitterIndex);
            var (tacticals, _) = this.entitiesDB.QueryEntities<Tactical>(tractorBeamEmitterId.EGID.groupID);

            ref TractorBeamEmitter tractorBeamEmitter = ref tractorBeamEmitters[tractorBeamEmitterIndex];
            ref Tactical tactical = ref tacticals[tractorBeamEmitterIndex];

            if (tractorBeamEmitter.TargetId.VhId.Value != data.TargetVhId.Value)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TargetVhId has changed from {OldTargetId} to {NewTargetId}", nameof(TractorBeamEmitterDeactivationEngine), nameof(Process), nameof(TractorBeamEmitter_Deactivate), data.TargetVhId.Value, tractorBeamEmitter.TargetId.VhId.Value);

                return;
            }

            // Flush just in case the target was created in the same frame
            _entities.Flush();
            this.entitiesDB.QueryEntity<Tractorable>(tractorBeamEmitter.TargetId.EGID).IsTractored = false;


            if (data.AttachTo is not null)
            {
                EntityId socketsId = _entities.GetId(data.AttachTo.Value.NodeVhId);
                ref Sockets sockets = ref this.entitiesDB.QueryEntity<Sockets>(socketsId.EGID);
                ref Socket socket = ref sockets.Items[data.AttachTo.Value.Index];

                _socketService.Attach(
                    socket: ref socket,
                    treeId: tractorBeamEmitter.TargetId);
            }


            // Ensure the emitter is deactivated no matter what
            tractorBeamEmitter.Active = false;
            tractorBeamEmitter.TargetId = default;
            tactical.RemoveUse();
        }
    }
}
