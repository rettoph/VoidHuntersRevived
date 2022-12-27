using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Simulations.EventData;
using VoidHuntersRevived.Library.Simulations.EventTypes;

namespace VoidHuntersRevived.Library.Factories
{
    public interface ITickFactory
    {
        void Enqueue(ISimulationEventData data);

        Tick Create(int id);
    }
}
