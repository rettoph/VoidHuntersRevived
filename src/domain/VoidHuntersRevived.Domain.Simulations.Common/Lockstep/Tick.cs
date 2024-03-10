using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Core.Utilities;
using VoidHuntersRevived.Common.Simulations.Enums;

namespace VoidHuntersRevived.Common.Simulations.Lockstep
{
    public sealed class Tick
    {
        public readonly int Id;
        public readonly EventDto[] Events;
        public readonly VhId Hash;
        public readonly TickQueue Queue;

        private Tick(int id, EventDto[] events, TickQueue queue)
        {
            this.Id = id;
            this.Events = events;
            this.Hash = HashBuilder<Tick, int>.Instance.Calculate(id);
            this.Queue = queue;

            foreach (EventDto @event in events)
            {
                Hash = Hash.Create(@event.Id);
            }
        }

        public override string ToString()
        {
            return $"Id = {Id}, Events: {this.Events.Length}, Hash = {Hash}";
        }

        public Tick Next(EventDto[] events, TickQueue queue = TickQueue.One)
        {
            return new Tick(Id + 1, events, queue);
        }

        public static Tick First(EventDto[] events, TickQueue queue = TickQueue.One)
        {
            return new Tick(0, events, queue);
        }

        public static Tick Empty(int id, TickQueue queue = TickQueue.One)
        {
            return new Tick(id, Array.Empty<EventDto>(), queue);
        }

        public static Tick Create(int id, EventDto[] events, TickQueue queue = TickQueue.One)
        {
            return new Tick(id, events, queue);
        }
    }
}
