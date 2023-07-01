using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Events.Services
{
    public interface IEventPublishingService
    {
        void Publish(EventDto @event);
        void Revert(EventDto @event);
    }
}
