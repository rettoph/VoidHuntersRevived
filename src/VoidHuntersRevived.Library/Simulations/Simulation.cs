using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Extensions;
using Guppy.Common.Implementations;
using Guppy.Common.Providers;
using Guppy.ECS.Providers;
using Microsoft.Extensions.Options;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Simulations
{
    [GuppyFilter<GameGuppy>()]
    public abstract class Simulation
    {
        public abstract SimulationType Type { get; }
        public abstract AetherWorld Aether { get; }
    }
}
