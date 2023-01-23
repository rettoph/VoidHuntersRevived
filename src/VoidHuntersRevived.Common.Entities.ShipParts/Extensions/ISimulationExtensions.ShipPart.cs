using Microsoft.Extensions.DependencyInjection;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Extensions
{
    public static partial class ISimulationExtensions
    {
        public static Entity CreateShipPart(this ISimulation simulation, ParallelKey key, string configuration)
        {
            simulation.PublishEvent(new CreateShipPart(key, configuration));
            return simulation.GetEntity(key);
        }
    }
}
