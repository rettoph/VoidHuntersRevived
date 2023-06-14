using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public static class ISimulationExtensions
    {
        public static void Publish(this ISimulation simulation, VhId id, IEventData data)
        {
            simulation.Publish(new EventDto()
            {
                Id = id,
                Data = data
            });
        }
    }
}
