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
    internal class DestroyNode : IEventData
    {
        public static VhId NameSpace = new VhId("762D9FAC-46E4-4765-8124-0D2CC582E526");

        public required VhId TreeId { get; init; }
        public required VhId NodeId { get; init; }
    }
}
