using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Enums;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulationEventListener<TEventData>
        where TEventData : ISimulationEventData
    {
        SimulationEventResult Process(ISimulation simulation, TEventData data);

        //void Rollback(TInput input, ISimulation simulation);
    }
}
