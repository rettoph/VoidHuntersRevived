using Guppy.Core.Common.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    public sealed class SocketIdsEngine(
        IEntityQueryService entityQueryService,
        IEntitySpawnService entitySpawnService,
        INodeSocketService socketService,
        ILogger logger) : StrategyEngine,
        IOnDespawnEngine<Sockets>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly INodeSocketService _socketService = socketService;
        private readonly ILogger _logger = logger;

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Sockets> sockets)
        {
            for (int i = 0; i < sockets.Component.Items.count; i++)
            {
                var filter = _socketService.GetCouplingFilter(nodeId: sockets.EntityId, socketIndex: (byte)i);
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
