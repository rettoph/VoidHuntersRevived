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
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Ships.Events;

namespace VoidHuntersRevived.Game.Ships.Engines
{
    [AutoLoad]
    internal sealed class TractorBeamEmitterAttachmentEngine : BasicEngine,
        IEventEngine<TractorBeamEmitter_TryAttach>
    {
        private readonly IEntityService _entities;
        private readonly ISocketService _sockets;
        private readonly ILogger _logger;

        public TractorBeamEmitterAttachmentEngine(IEntityService entities, ISocketService socketService, ILogger logger)
        {
            _entities = entities;
            _sockets = socketService;
            _logger = logger;
        }

        public void Process(VhId eventId, TractorBeamEmitter_TryAttach data)
        {
            // TODO: Ensure that the tractor beam owns the socketNode in question.
            // This just trusts the tractor beam is allowed to attach to the given node

            this.Simulation.Publish(eventId, new TractorBeamEmitter_TryDeactivate()
            {
                ShipVhId = data.ShipVhId,
                TargetVhId = data.TargetVhId
            });

            _entities.Flush();

            if(_sockets.TryGetSocketNode(data.SocketVhId, out SocketNode socketNode) && _entities.TryGetId(data.TargetVhId, out EntityId targetId))
            {
                _sockets.Attach(socketNode, _entities.QueryById<Tree>(targetId));
            }
        }
    }
}
