using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Game.Pieces.Events
{
    public class CreateNode : IEventData
    {
        public static VhId NameSpace = new VhId("9BAFF71B-8F89-4B96-8FEC-7D6CA3C37B6B");

        public required VhId NodeId { get; init; }
        public required VhId TreeId { get; init; }
    }
}
