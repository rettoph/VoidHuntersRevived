using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulationEventListener<TSimulationEventData>
        where TSimulationEventData : class
    {
        void Process(ISimulationEvent<TSimulationEventData> @event);

        //void Rollback(TInput input, ISimulation simulation);
    }
}
