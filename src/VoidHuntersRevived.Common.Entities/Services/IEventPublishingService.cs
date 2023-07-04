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
        event OnEventDelegate<EventDto>? OnVerifiedEvent;

        /// <summary>
        /// Publish an event to be processed
        /// </summary>
        /// <param name="event"></param>
        /// <param name="validity"></param>
        void Publish(EventDto @event, EventValidity validity);

        /// <summary>
        /// Revert a previously published event
        /// </summary>
        /// <param name="eventId"></param>
        void Revert(VhId eventId);

        /// <summary>
        /// Mark a previously published event as verified
        /// </summary>
        /// <param name="eventId"></param>
        void Validate(VhId eventId);
    }
}
