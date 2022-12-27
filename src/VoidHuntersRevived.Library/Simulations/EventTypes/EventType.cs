using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Simulations.EventData;

namespace VoidHuntersRevived.Library.Simulations.EventTypes
{
    public class EventType : Message
    {
        private readonly IEnumerable<ISimulationEventData> _eventData;
        private int? _count;

        public IEnumerable<ISimulationEventData> EventData => _eventData;

        public int Count => _count ??= _eventData.Count();

        internal EventType(IEnumerable<ISimulationEventData> eventData)
        {
            _eventData = eventData;
        }
    }
}
