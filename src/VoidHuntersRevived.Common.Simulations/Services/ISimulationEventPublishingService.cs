using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Enums;

namespace VoidHuntersRevived.Common.Simulations.Services
{
    public interface ISimulationEventPublishingService
    {
        SimulationEventResult Publish(ISimulation simulation, ISimulationEventData data);
    }
}
