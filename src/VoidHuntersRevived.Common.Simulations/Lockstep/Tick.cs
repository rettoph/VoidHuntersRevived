using Guppy.Common;

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

        public EventId Hash()
        {
            EventId hash = EventId.Empty.Step((ulong)this.Id);


            foreach(EventDto eventData in _events)
            {
                hash = hash.Merge(eventData.Key);
            }

            return hash;
        }
    }
}
