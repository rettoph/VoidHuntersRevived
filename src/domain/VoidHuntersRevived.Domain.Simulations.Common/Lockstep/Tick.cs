using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Simulations.Common.Lockstep
{
    public sealed class Tick
    {
        public readonly int Id;
        public readonly EventDto[] Events;
        public readonly VhId Hash;

        private Tick(int id, EventDto[] events)
        {
            this.Id = id;
            this.Events = events;
            this.Hash = HashBuilder<Tick, int>.Instance.Calculate(id);

            foreach (EventDto @event in events)
            {
                this.Hash = this.Hash.Create(@event.Id);
            }
        }

        public override string ToString()
        {
            return $"Id = {this.Id}, Events: {this.Events.Length}, Hash = {this.Hash}";
        }

        public Tick Next(EventDto[] events)
        {
            return new(this.Id + 1, events);
        }

        public static Tick First(EventDto[] events)
        {
            return new(0, events);
        }

        public static Tick Empty(int id)
        {
            return new(id, []);
        }

        public static Tick Create(int id, EventDto[] events)
        {
            return new(id, events);
        }
    }
}