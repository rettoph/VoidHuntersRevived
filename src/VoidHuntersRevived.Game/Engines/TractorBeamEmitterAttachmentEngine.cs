using Guppy.Attributes;
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
        IEventEngine<TractorBeamEmitter_Attach>
    {
        private readonly IEntityService _entities;
        private readonly ISocketService _socketService;

        public TractorBeamEmitterAttachmentEngine(IEntityService entities, ISocketService socketService)
        {
            _entities = entities;
            _socketService = socketService;
        }

        public void Process(VhId eventId, TractorBeamEmitter_Attach data)
        {
            // EntityId socketsId = _entities.GetId(data.SocketVhId.NodeVhId);
            // ref Sockets sockets = ref this.entitiesDB.QueryEntity<Sockets>(socketsId.EGID);
            // ref Socket socket = ref sockets.Items[data.SocketVhId.Index];
            // 
            // _socketService.Attach(
            //     socket: ref socket,
            //     treeId: tractorBeamEmitter.TargetId);
        }
    }
}
