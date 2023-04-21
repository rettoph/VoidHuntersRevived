using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Factories
{
    [PeerTypeFilter(PeerType.Server | PeerType.None)]
    internal sealed class DefaultTickFactory : ITickFactory
    {
        private IList<EventDto> _events;

        public DefaultTickFactory()
        {
            _events = new List<EventDto>();
        }

        public void Enqueue(EventDto @event)
        {
            _events.Add(@event);
        }

        public Tick Create(int id)
        {
            if (_events.Count == 0)
            {
                return Tick.Create(id, Enumerable.Empty<EventDto>());
            }

            var tick = Tick.Create(id, _events);
            _events = new List<EventDto>();

            return tick;
        }

        public void Reset()
        {
            _events.Clear();
        }
    }
}
