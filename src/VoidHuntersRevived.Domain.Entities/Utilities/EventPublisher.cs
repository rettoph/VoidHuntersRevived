using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;
using Serilog;
using VoidHuntersRevived.Domain.Entities.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;

namespace VoidHuntersRevived.Domain.Entities.Utilities
{
    internal abstract class EventPublisher
    {
        public abstract void Publish(EventDto @event);
        public abstract void Revert(EventDto @event);
    }
    internal class EventPublisher<T> : EventPublisher
        where T : class, IEventData
    {
        private readonly IEventEngine<T>[] _subscribers;
        private readonly IRevertEventEngine<T>[] _reverters;
        private readonly ILogger _logger;

        public EventPublisher(ILogger logger, List<IEventEngine> subscribers)
        {
            _logger = logger;
            _subscribers = subscribers.OfType<IEventEngine<T>>().ToArray();
            _reverters = subscribers.OfType<IRevertEventEngine<T>>().ToArray();
        }

        public override void Publish(EventDto @event)
        {
            this.Publish(@event.Id, Unsafe.As<T>(@event.Data));
        }

        private void Publish(in VhId id, T data)
        {
            _logger.Verbose("{ClassName}::{MethodName} - Publishing Event {EventId} {EventType}", nameof(EventPublishingService), nameof(Publish), id.Value, typeof(T).Name);

            foreach (IEventEngine<T> subscriber in _subscribers)
            {
                subscriber.Process(id, data);
            }
        }

        public override void Revert(EventDto @event)
        {
            this.Revert(@event.Id, Unsafe.As<T>(@event.Data));
        }

        private void Revert(in VhId id, T data)
        {
            if (_reverters.Length == 0)
            {
                return;
            }

            _logger.Verbose("{ClassName}::{MethodName} - Reverting Event {EventId} {EventType}", nameof(EventPublishingService), nameof(Revert), id.Value, typeof(T).Name);
            foreach (IRevertEventEngine<T> reverter in _reverters)
            {
                reverter.Revert(id, data);
            }
        }
    }
}
