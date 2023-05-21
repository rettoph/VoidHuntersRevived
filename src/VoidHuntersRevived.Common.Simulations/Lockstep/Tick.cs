using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Common.Simulations.Lockstep
{
    public sealed class Tick : Message<Tick>
    {
        private readonly IEnumerable<SimulationEventData> _events;
        private int? _count;

        public IEnumerable<SimulationEventData> Events => _events;

        public int Count => _count ??= _events.Count();

        public int Id { get; }

        internal Tick(int id, IEnumerable<SimulationEventData> events)
        {
            _events = events;

            this.Id = id;
        }

        public static Tick Empty(int id)
        {
            return new Tick(id, Enumerable.Empty<SimulationEventData>());
        }

        public static Tick Create(int id, IEnumerable<SimulationEventData> events)
        {
            return new Tick(id, events);
        }

        public ParallelKey Hash()
        {
            ParallelKey hash = ParallelKey.Empty.Step((ulong)this.Id);


            foreach(SimulationEventData eventData in _events)
            {
                hash = hash.Merge(eventData.Key);
            }

            return hash;
        }
    }
}
