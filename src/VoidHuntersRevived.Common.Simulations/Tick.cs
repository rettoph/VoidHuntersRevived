using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public sealed class Tick
    {
        public readonly uint Id;
        public readonly EventDto[] Events;

        private Tick(uint id, EventDto[] events)
        {
            this.Id = id;
            this.Events = events;
        }

        public Tick Next(params EventDto[] events)
        {
            return new Tick(this.Id + 1, events);
        }

        public static Tick First(params EventDto[] events)
        {
            return new Tick(0, events);
        }
    }
}
