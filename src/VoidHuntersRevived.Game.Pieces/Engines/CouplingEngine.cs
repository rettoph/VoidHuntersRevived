using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Pieces.Events;
using VoidHuntersRevived.Common.Pieces;

namespace VoidHuntersRevived.Game.Pieces.Engines
{
    [AutoLoad]
    internal sealed class CouplingEngine : BasicEngine,
        IReactOnAddEx<Coupling>
    {
        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Coupling> entities, ExclusiveGroupStruct groupID)
        {
            var (couplings, _, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                Coupling coupling = couplings[index];

                if(coupling.SocketId == SocketId.Empty)
                {
                    continue;
                }

                throw new NotImplementedException();
            }
        }
    }
}
