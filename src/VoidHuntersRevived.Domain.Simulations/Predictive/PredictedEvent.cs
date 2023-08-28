using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Domain.Simulations.Predictive.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    internal class PredictedEvent
    {
        public static readonly TimeSpan Lifetime = TimeSpan.FromSeconds(5);

        public readonly EventDto Event;
        public PredictedEventStatus Status;
        public DateTime PublishedAt;

        public PredictedEvent(EventDto @event)
        {
            Event = @event;
            PublishedAt = DateTime.Now;
            Status = PredictedEventStatus.Unconfirmed;
        }

        public bool Expired => DateTime.Now - PublishedAt >= Lifetime;
    }
}
