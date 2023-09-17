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

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    internal sealed class CouplingEngine : BasicEngine, IReactOnAddEx<Coupling>, IReactOnAddAndRemoveEx<Coupling>
    {
        private readonly ISocketService _sockets;
        private readonly ILogger _logger;

        public CouplingEngine(ISocketService sockets, ILogger logger)
        {
            _sockets = sockets;
            _logger = logger;
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Coupling> entities, ExclusiveGroupStruct groupID)
        {
            var (couplings, ids, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                ref Coupling coupling = ref couplings[index];

                if(coupling.SocketId == default)
                {
                    continue;
                }

                ref var filter = ref _sockets.GetCouplingFilter(coupling.SocketId);
                filter.Add(ids[index], groupID, index);
            }
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<Coupling> entities, ExclusiveGroupStruct groupID)
        {
            var (couplings, ids, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                ref Coupling coupling = ref couplings[index];

                if (coupling.SocketId == default)
                {
                    continue;
                }

                ref var filter = ref _sockets.GetCouplingFilter(coupling.SocketId);
                filter.Remove(ids[index], groupID);
            }
        }
    }
}
