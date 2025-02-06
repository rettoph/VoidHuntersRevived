using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Pieces.Systems
{
    public sealed class SocketIdsSystem(
        IEntityQueryService entityQueryService,
        IEntitySpawnService entitySpawnService,
        INodeSocketService socketService) : StrategySystem,
        IOnDespawnEngine<Sockets>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly INodeSocketService _socketService = socketService;

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Sockets> sockets)
        {
            for (int i = 0; i < sockets.Component.Items.count; i++)
            {
                var filter = this._socketService.GetCouplingFilter(nodeLocalId: sockets.LocalId, socketIndex: (byte)i);
                foreach (var (indices, groupId) in filter)
                {
                    var (localIds, _) = this._entityQueryService.QueryEntities<EntityLocalId>(groupId);

                    for (int j = 0; j < indices.count; j++)
                    {
                        this._entitySpawnService.Despawn(sourceEventId, localIds[indices[j]]);
                    }
                }
            }
        }
    }
}