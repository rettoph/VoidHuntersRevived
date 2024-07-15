using Guppy.Core.Common.Attributes;
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
    internal sealed class SocketIdsEngine : StrategyEngine,
        IOnDespawnEngine<Sockets<SocketId>>
    {
        private readonly IEntityQueryService _entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService;
        private readonly ISocketService _socketService;
        private readonly ILogger _logger;

        public SocketIdsEngine(
            IEntityQueryService entityQueryService,
            IEntitySpawnService entitySpawnService,
            ISocketService socketService,
            ILogger logger)
        {
            _entityQueryService = entityQueryService;
            _entitySpawnService = entitySpawnService;
            _socketService = socketService;
            _logger = logger;
        }

        public void OnDespawn(VhId sourceEventId, IEntityType type, EntityId id, ref Sockets<SocketId> sockets, in GroupIndex groupIndex)
        {
            for (int i = 0; i < sockets.Items.count; i++)
            {
                var filter = _socketService.GetCouplingFilter(sockets.Items[i]);
                foreach (var (indices, groupId) in filter)
                {
                    var (entityIds, _) = _entityQueryService.QueryEntities<EntityId>(groupId);

                    for (int j = 0; j < indices.count; j++)
                    {
                        _entitySpawnService.Despawn(sourceEventId, entityIds[indices[j]]);
                    }
                }
            }
        }
    }
}
