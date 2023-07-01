using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Events;

namespace VoidHuntersRevived.Game.Pieces.Events
{
    public class RemoveNodeFromTree : IEventData
    {
        public static VhId NameSpace = new VhId("b364834f-c048-41e8-8400-4a0cdc71eee2");

        public required VhId NodeId { get; init; }
    }
}
