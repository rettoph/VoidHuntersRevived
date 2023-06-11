﻿using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using Guppy.Network.Identity;
using Guppy.Resources.Providers;
using LiteNetLib;
using Microsoft.Xna.Framework;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Factories;
using VoidHuntersRevived.Common.Physics.Factories;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [GuppyFilter<IGameGuppy>()]
    internal abstract class LockstepSimulation : Simulation, ISimulation
    {
        public LockstepSimulation(
            IWorldFactory worldFactory,
            ISpaceFactory spaceFactory) : base(SimulationType.Lockstep, worldFactory, spaceFactory)
        {
        }

        public override void Publish(EventDto data)
        {
            this.publisher.Publish(data);
        }
    }
}
