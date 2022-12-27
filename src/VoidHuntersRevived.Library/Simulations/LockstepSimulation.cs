﻿using Guppy.Common;
using Guppy.Common.Providers;
using Guppy.ECS.Providers;
using Guppy.Network;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Extensions.Options;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Simulations;
using VoidHuntersRevived.Library.Simulations.EventData;
using VoidHuntersRevived.Library.Simulations.EventTypes;

namespace VoidHuntersRevived.Library.Games
{
    public sealed class LockstepSimulation : Simulation
    {
        public override SimulationType Type => SimulationType.Lockstep;

        public override AetherWorld Aether { get; } = new AetherWorld(Vector2.Zero);

        public LockstepSimulation()
        {
        }
    }
}
