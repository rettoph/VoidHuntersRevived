using Guppy.Core.Common.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    [SequenceGroup<EngineSequence>(EngineSequence.Group03)]
    internal sealed class CouplingEngine : StrategyEngine,
        IOnSpawnEngine<Coupling>,
        IOnDespawnEngine<Coupling>
    {
        private readonly ISocketService _socketService;
        private readonly ILogger _logger;

        public CouplingEngine(ISocketService socketService, ILogger logger)
        {
            _socketService = socketService;
            _logger = logger;
        }

        public void OnSpawn(VhId sourceEventId, IEntityType type, EntityId id, ref Coupling coupling, in GroupIndex groupIndex)
        {
            if (coupling.SocketId == default)
            {
                return;
            }

            ref var filter = ref _socketService.GetCouplingFilter(coupling.SocketId);
            filter.Add(in id, in groupIndex);
        }

        public void OnDespawn(VhId sourceEventId, IEntityType type, EntityId id, ref Coupling coupling, in GroupIndex groupIndex)
        {
            if (coupling.SocketId == default)
            {
                return;
            }

            ref var filter = ref _socketService.GetCouplingFilter(coupling.SocketId);
            filter.Remove(in id);
        }
    }
}
