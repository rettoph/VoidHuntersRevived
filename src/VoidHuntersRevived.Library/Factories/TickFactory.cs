using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Factories
{
    internal sealed class TickFactory : ITickFactory
    {
        private IList<ISimulationEvent> _datum;

        public TickFactory()
        {
            _datum = new List<ISimulationEvent>();
        }

        public void Enqueue(ISimulationEvent data)
        {
            _datum.Add(data);
        }

        public Tick Create(int id)
        {
            if(_datum.Count == 0)
            {
                return new Tick(id, Enumerable.Empty<ISimulationEvent>());
            }

            var tick = new Tick(id, _datum);
            _datum = new List<ISimulationEvent>();

            return tick;
        }
    }
}
