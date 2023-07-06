using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities
{
    internal class PublishedEvent
    {
        public static readonly TimeSpan Lifetime = TimeSpan.FromSeconds(5);

        public readonly EventDto Event;
        public PublishedEventStatus Status;
        public DateTime PublishedAt;

        public PublishedEvent(EventDto @event)
        {
            this.Event = @event;
            this.PublishedAt = DateTime.Now;
            this.Status = PublishedEventStatus.Unconfirmed;
        }

        public bool Expired => DateTime.Now - this.PublishedAt >= Lifetime;
    }
}
