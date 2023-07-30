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
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Events;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class TractorBeamEmitterAttachmentEngine : BasicEngine,
        IEventEngine<TractorBeamEmitter_TryAttach>
    {
        private readonly IEntityService _entities;
        private readonly ISocketService _socketService;
        private readonly ILogger _logger;

        public TractorBeamEmitterAttachmentEngine(IEntityService entities, ISocketService socketService, ILogger logger)
        {
            _entities = entities;
            _socketService = socketService;
            _logger = logger;
        }

        public void Process(VhId eventId, TractorBeamEmitter_TryAttach data)
        {
            // TODO: Ensure that the tractor beam owns the socketNode in question.
            _socketService.Attach(
                socketVhId: data.SocketVhId,
                treeVhId: data.TargetVhId);
        }
    }
}
