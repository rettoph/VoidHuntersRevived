﻿using MonoGame.Extended.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static partial class ISimulationExtensions
    {
        public static Entity CreatePilot(this ISimulation simulation, ParallelKey key)
        {
            var pilot = simulation.CreateEntity(key);
            pilot.Attach(new Piloting());

            return pilot;
        }
    }
}
