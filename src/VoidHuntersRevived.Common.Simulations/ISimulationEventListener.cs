using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulationEventListener<TSimulationEventData> : ISubscriber<ISimulationEvent<TSimulationEventData>>
        where TSimulationEventData : class
    {
        //void Rollback(TInput input, ISimulation simulation);
    }
}
