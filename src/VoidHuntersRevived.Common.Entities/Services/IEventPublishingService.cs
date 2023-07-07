using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEventPublishingService
    {
        /// <summary>
        /// Invoked when an event is published
        /// </summary>
        event OnEventDelegate<EventDto>? OnEvent;

        /// <summary>
        /// Publish an event to be processed
        /// </summary>
        /// <param name="event"></param>
        void Publish(EventDto @event);

        /// <summary>
        /// Mark a previously published event as confirmed
        /// </summary>
        /// <param name="eventId"></param>
        void Confirm(EventDto @event);

        /// <summary>
        /// Prune any events, reverting expired published events.
        /// </summary>
        void Revert();

        /// <summary>
        /// Confirm all unconfirmed events.
        /// </summary>
        void Confirm();
    }
}
