using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Pieces.Events;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Services;
using Serilog;
using Guppy.Attributes;

namespace VoidHuntersRevived.Game.Pieces.Engines
{
    [AutoLoad]
    internal sealed class SocketsEngine : BasicEngine, IReactOnRemoveEx<Sockets>
    {
        private readonly IEntityService _entities;
        private readonly ISocketService _sockets;
        private readonly ILogger _logger;

        public SocketsEngine(IEntityService entities, ISocketService sockets, ILogger logger)
        {
            _entities = entities;
            _sockets = sockets;
            _logger = logger;
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<Sockets> entities, ExclusiveGroupStruct groupID)
        {
            var (socketses, ids, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                Sockets sockets = socketses[index];
                for (int i=0; i < sockets.Items.count; i++)
                {
                    var filter = _sockets.GetCouplingFilter(sockets.Items[i].Id);
                    foreach (var (indices, groupId) in filter)
                    {
                        var (entityIds, _) = _entities.QueryEntities<EntityId>(groupId);

                        for (int j = 0; j < indices.count; j++)
                        {
                            _entities.Despawn(entityIds[indices[j]]);
                        }
                    }
                }
            }
        }
    }
}
