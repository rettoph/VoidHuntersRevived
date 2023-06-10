using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class SetTacticalTarget
    {
        public required EventId TacticalKey { get; init; }
        public required FixVector2 Target { get; init; }
    }
}
