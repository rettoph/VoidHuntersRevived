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
    internal sealed class TickFactory : ITickFactory
    {
        private IList<ISimulationEventData> _datum;

        public TickFactory()
        {
            _datum = new List<ISimulationEventData>();
        }

        public void Enqueue(ISimulationEventData data)
        {
            _datum.Add(data);
        }

        public Tick Create(int id)
        {
            if(_datum.Count == 0)
            {
                return new Tick(id, Enumerable.Empty<ISimulationEventData>());
            }

            var tick = new Tick(id, _datum);
            _datum = new List<ISimulationEventData>();

            return tick;
        }
    }
}
