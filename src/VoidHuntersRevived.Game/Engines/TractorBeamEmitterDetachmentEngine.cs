using Guppy.Attributes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Events;

namespace VoidHuntersRevived.Game.Engines
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
            // TODO: Ensure that the tractor beam owns the socketNode in question.
            // This just trusts the tractor beam is allowed to attach to the given node

            this.Simulation.Publish(eventId, new TractorBeamEmitter_TryDeactivate()
            {
                ShipVhId = data.ShipVhId,
                TargetVhId = data.TargetVhId
            });

            _socketService.Detach(data.TargetVhId);
        }
    }
}
