using Guppy.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    internal sealed class SocketIdsEngine : BasicEngine,
        IOnDespawnEngine<Sockets<SocketId>>
    {
        private readonly IEntityService _entities;
        private readonly ISocketService _sockets;
        private readonly ILogger _logger;

        public SocketIdsEngine(IEntityService entities, ISocketService sockets, ILogger logger)
        {
            _entities = entities;
            _sockets = sockets;
            _logger = logger;
        }

        public void OnDespawn(VhId sourceEventId, EntityId id, ref Sockets<SocketId> sockets, in GroupIndex groupIndex)
        {
            for (int i = 0; i < sockets.Items.count; i++)
            {
                var filter = _sockets.GetCouplingFilter(sockets.Items[i]);
                foreach (var (indices, groupId) in filter)
                {
                    var (entityIds, _) = _entities.QueryEntities<EntityId>(groupId);

                    for (int j = 0; j < indices.count; j++)
                    {
                        _entities.Despawn(sourceEventId, entityIds[indices[j]]);
                    }
                }
            }
        }
    }
}
