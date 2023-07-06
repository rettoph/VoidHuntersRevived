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
        public static VhId NameSpace = new VhId("10ce7a16-ac26-4627-9489-df535f8f81a6");

        public required VhId NodeId { get; init; }
        public required VhId TreeId { get; init; }
    }
}
