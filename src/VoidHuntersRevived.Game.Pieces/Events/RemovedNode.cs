using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Pieces.Events
{
    public class RemovedNode : IEventData
    {
        public VhId NodeId;
        public VhId TreeId;
    }
}
