using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class SetTacticalTarget
    {
        public required ParallelKey TacticalKey { get; init; }
        public required AetherVector2 Target { get; init; }
    }
}
