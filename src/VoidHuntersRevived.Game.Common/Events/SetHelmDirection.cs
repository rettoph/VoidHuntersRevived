using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Game.Common.Enums;

namespace VoidHuntersRevived.Game.Common.Events
{
    public class SetHelmDirection : IEventData
    {
        public required Direction Which { get; init; }
        public required bool Value { get; init; }
    }
}
