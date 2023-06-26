using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Game.Events
{
    public class TryCreateEntity : IEventData
    {
        public static readonly VhId NameSpace = new VhId("6a7a5aed-2cfe-4c85-8c24-421a82f1d738");

        public required VhId EntityVhId { get; init; }
        public required EntityType Type { get; init; }
        public required EntityInitializerDelegate Initializer { get; init; }
    }
}
