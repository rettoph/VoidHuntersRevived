using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Pieces.Systems
{
    public sealed class CouplingSystem(INodeSocketService socketService) : StrategySystem,
        IOnSpawnEngine<Coupling>,
        IOnDespawnEngine<Coupling>
    {
        private readonly INodeSocketService _socketService = socketService;

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group03)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Coupling> entity)
        {
            if (entity.Component.SocketId == default)
            {
                return;
            }

            ref var filter = ref this._socketService.GetCouplingFilter(entity.Component.SocketId);
            filter.Add(in entity.LocalId, in entity.Index);
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Coupling> coupling)
        {
            if (coupling.Component.SocketId == default)
            {
                return;
            }

            ref var filter = ref this._socketService.GetCouplingFilter(coupling.Component.SocketId);
            filter.Remove(in coupling.LocalId);
        }
    }
}