using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Pieces.Events;

namespace VoidHuntersRevived.Game.Pieces.Engines
{
    [AutoLoad]
    internal sealed class NodeEngine : BasicEngine,
        IEventEngine<DestroyNode>
    {
        public void Process(VhId eventId, DestroyNode data)
        {
            // this.Simulation.Entities.Destroy(data.NodeId);
        }
    }
}
