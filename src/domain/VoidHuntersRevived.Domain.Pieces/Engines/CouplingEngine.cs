using Guppy.Attributes;
using Serilog;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    internal sealed class CouplingEngine : BasicEngine,
        IOnSpawnEngine<Coupling>,
        IOnDespawnEngine<Coupling>
    {
        private readonly ISocketService _sockets;
        private readonly ILogger _logger;

        public CouplingEngine(ISocketService sockets, ILogger logger)
        {
            _sockets = sockets;
            _logger = logger;
        }

        public void OnSpawn(VhId sourceEventId, EntityId id, ref Coupling coupling, in GroupIndex groupIndex)
        {
            if (coupling.SocketId == default)
            {
                return;
            }

            ref var filter = ref _sockets.GetCouplingFilter(coupling.SocketId);
            filter.Add(in id, in groupIndex);
        }

        public void OnDespawn(VhId sourceEventId, EntityId id, ref Coupling coupling, in GroupIndex groupIndex)
        {
            if (coupling.SocketId == default)
            {
                return;
            }

            ref var filter = ref _sockets.GetCouplingFilter(coupling.SocketId);
            filter.Remove(in id);
        }
    }
}
