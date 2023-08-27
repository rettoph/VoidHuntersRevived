using Guppy.Attributes;
using Serilog;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Ships.Events;

namespace VoidHuntersRevived.Game.Ships.Engines
{
    [AutoLoad]
    internal sealed class TractorBeamEmitterDetachmentEngine : BasicEngine,
        IEventEngine<TractorBeamEmitter_TryDetach>
    {
        private readonly IEntityService _entities;
        private readonly ISocketService _socketService;
        private readonly ILogger _logger;

        public TractorBeamEmitterDetachmentEngine(IEntityService entities, ISocketService socketService, ILogger logger)
        {
            _entities = entities;
            _socketService = socketService;
            _logger = logger;
        }

        public void Process(VhId eventId, TractorBeamEmitter_TryDetach data)
        {
            if(!_entities.TryGetId(data.TargetVhId, out EntityId targetId))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Unable to locate id {TargetId}", nameof(TractorBeamEmitterDetachmentEngine), nameof(Process), nameof(TractorBeamEmitter_TryDetach), data.TargetVhId);

                return;
            }

            if(!_socketService.TryDetach(targetId, this.InitDetachedTree, out EntityId cloneId))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Unable to locate detatch {TargetId}", nameof(TractorBeamEmitterDetachmentEngine), nameof(Process), nameof(TractorBeamEmitter_TryDetach), data.TargetVhId);

                return;
            }

            // EntityId tractorBeamEmitterId = _entities.GetId(data.ShipVhId);
            // ref TractorBeamEmitter tractorBeamEmitter = ref _entities.QueryById<TractorBeamEmitter>(tractorBeamEmitterId, out GroupIndex groupIndex);
            // ref Tactical tactical = ref _entities.QueryByGroupIndex<Tactical>(groupIndex);
            // 
            // if (tractorBeamEmitter.Active)
            // {
            //     this.Simulation.Publish(eventId, new TractorBeamEmitter_TryDeactivate()
            //     {
            //         ShipVhId = data.ShipVhId,
            //         TargetVhId = tractorBeamEmitter.TargetId.VhId
            //     });
            // }
            // 
            // tractorBeamEmitter.TargetId = cloneId;
            // tractorBeamEmitter.Active = true;
            // tactical.AddUse();
            // 
            // _logger.Verbose("{ClassName}::{MethodName}<{GenericTypeName}> - TractorBeam {TractorBeamId} has detached and selected {TargetId}", nameof(TractorBeamEmitterActivationEngine), nameof(Process), nameof(TractorBeamEmitter_TryDetach), tractorBeamEmitterId.VhId.Value, tractorBeamEmitter.TargetId.VhId.Value);
        }

        private void InitDetachedTree(IEntityService entities, ref EntityInitializer initializer, in EntityId id)
        {
            // initializer.Init<Tractorable>(new Tractorable()
            // {
            //     IsTractored = true
            // });
        }
    }
}
