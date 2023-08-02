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
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
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
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Unable to locate id {NodeId}", nameof(TractorBeamEmitterDetachmentEngine), nameof(Process), nameof(TractorBeamEmitter_TryDetach), data.TargetVhId);

                return;
            }

            if(!this.entitiesDB.TryGetEntity<Coupling>(targetId.EGID, out Coupling coupling))
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Unable to locate coupling for entity {NodeId}", nameof(TractorBeamEmitterDetachmentEngine), nameof(Process), nameof(TractorBeamEmitter_TryDetach), data.TargetVhId);
            }

            _socketService.Detach(coupling.SocketId.VhId);
        }
    }
}
