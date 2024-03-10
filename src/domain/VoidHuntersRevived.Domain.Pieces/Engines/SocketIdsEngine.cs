using Guppy.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

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
