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
        public static VhId NameSpace = new VhId("50b7c7fc-6d6c-40fe-9547-440c0c666f7e");

        public required VhId NodeId { get; init; }
    }
}
