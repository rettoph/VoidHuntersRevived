using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Events;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Game.Pieces.Events
{
    public class AddNodeToTree : IEventData
    {
        public static VhId NameSpace = new VhId("99a0fdab-25a9-4f06-a143-0da81a69ce99");

        public required VhId NodeId { get; init; }
        public required VhId TreeId { get; init; }
    }
}
