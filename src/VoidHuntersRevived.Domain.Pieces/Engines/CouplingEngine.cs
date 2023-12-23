using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using Serilog;
using VoidHuntersRevived.Common.Entities.Engines;
using System.Text.RegularExpressions;

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

        public void OnSpawn(EntityId id, ref Coupling coupling, in GroupIndex groupIndex)
        {
            if (coupling.SocketId == default)
            {
                return;
            }

            ref var filter = ref _sockets.GetCouplingFilter(coupling.SocketId);
            filter.Add(in id, in groupIndex);
        }

        public void OnDespawn(EntityId id, ref Coupling coupling, in GroupIndex groupIndex)
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
