using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Game.Events
{
    public class AddNodeToTree : IEventData
    {
        public VhId NodeId;
        public VhId TreeId;
    }
}
