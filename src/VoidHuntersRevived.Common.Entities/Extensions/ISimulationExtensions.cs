using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static partial class ISimulationExtensions
    {
        public static Entity CreateEntity(this ISimulation simulation, ParallelKey key)
        {
            simulation.PublishEvent(new CreateEntity(key));
            return simulation.GetEntity(key);
        }
    }
}
