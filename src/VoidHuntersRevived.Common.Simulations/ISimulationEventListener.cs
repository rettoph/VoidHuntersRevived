using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Enums;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulationEventListener<TSimulationEventData>
        where TSimulationEventData : class
    {
        SimulationEventResult Process(ISimulationEvent<TSimulationEventData> @event);

        //void Rollback(TInput input, ISimulation simulation);
    }
}
