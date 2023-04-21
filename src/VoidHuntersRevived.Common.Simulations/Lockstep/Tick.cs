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
        private readonly IEnumerable<EventDto> _events;
        private int? _count;

        public IEnumerable<EventDto> Events => _events;

        public int Count => _count ??= _events.Count();

        public int Id { get; }

        internal Tick(int id, IEnumerable<EventDto> events)
        {
            _events = events;

            this.Id = id;
        }

        public static Tick Empty(int id)
        {
            return new Tick(id, Enumerable.Empty<EventDto>());
        }

        public static Tick Create(int id, IEnumerable<EventDto> events)
        {
            return new Tick(id, events);
        }
    }
}
