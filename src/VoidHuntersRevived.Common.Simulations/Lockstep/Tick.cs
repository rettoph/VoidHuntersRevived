using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Simulations.Lockstep
{
    public sealed class Tick
    {
        public readonly int Id;
        public readonly EventDto[] Events;
        public readonly VhId Hash;

        private Tick(int id, EventDto[] events)
        {
            Id = id;
            Events = events;
            Hash = NameSpace<Tick>.Instance.Create(Id);

            foreach(EventDto @event in events)
            {
                Hash = Hash.Create(@event.Id);
            }
        }

        public Tick Next(params EventDto[] events)
        {
            return new Tick(Id + 1, events);
        }

        public static Tick First(params EventDto[] events)
        {
            return new Tick(0, events);
        }

        public static Tick Empty(int id)
        {
            return new Tick(id, Array.Empty<EventDto>());
        }

        public static Tick Create(int id, EventDto[] events)
        {
            return new Tick(id, events);
        }
    }
}
