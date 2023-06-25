using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Game.Events
{
    public class DestroyEntity : IEventData
    {
        public static readonly VhId NameSpace = new VhId("5674DF89-EF53-4A54-BDB6-B3D8BCBCF90D");

        public required VhId EntityId { get; init; }
    }
}
