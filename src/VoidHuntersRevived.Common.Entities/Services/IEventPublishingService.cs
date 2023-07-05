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
        /// Revert a previously published event
        /// </summary>
        /// <param name="eventId"></param>
        void Revert(VhId eventId);

        /// <summary>
        /// Mark a previously published event as confirmed
        /// </summary>
        /// <param name="eventId"></param>
        void Confirm(VhId eventId);

        /// <summary>
        /// Publish the event if not already published and mark as confirmed
        /// </summary>
        /// <param name="eventId"></param>
        void Confirm(EventDto @event);
    }
}
