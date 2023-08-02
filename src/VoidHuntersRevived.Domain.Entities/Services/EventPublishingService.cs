using Guppy.Common;
using Guppy.Common.Collections;
using Serilog;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public sealed class EventPublishingService : IEventPublishingService, IGetReadyEngine
    {
        private Dictionary<Type, EventPublisher> _publishers;
        private DictionaryQueue<VhId, PublishedEvent> _published;
        private readonly ILogger _logger;
        private readonly IEngineService _engines;

        public event OnEventDelegate<EventDto>? OnEvent;

        public EventPublishingService(
            ILogger logger,
            IEngineService engines)
        {
            _logger = logger;
            _engines = engines;
            _publishers = null!;
            _published = new DictionaryQueue<VhId, PublishedEvent>();
        }


        public void Ready()
        {
            Dictionary<Type, List<IEventEngine>> subscriptions = new Dictionary<Type, List<IEventEngine>>();
            foreach (IEventEngine system in _engines.OfType<IEventEngine>())
            {
                foreach (Type subscriberType in system.GetType().GetConstructedGenericTypes(typeof(IEventEngine<>)))
                {
                    if (!subscriptions.TryGetValue(subscriberType.GenericTypeArguments[0], out List<IEventEngine>? subSystems))
                    {
                        subscriptions[subscriberType.GenericTypeArguments[0]] = subSystems = new List<IEventEngine>();
                    }

                    subSystems.Add(system); ;
                }
            }

            _publishers = new Dictionary<Type, EventPublisher>();
            foreach ((Type type, List<IEventEngine> subscribers) in subscriptions)
            {
                Type publisherType = typeof(EventPublisher<>).MakeGenericType(type);
                EventPublisher publisher = (EventPublisher)Activator.CreateInstance(publisherType, new object[] { _logger, subscribers })!;
                _publishers.Add(type, publisher);
            }
        }

        public void Publish(VhId sender, IEventData data)
        {
            this.Publish(new EventDto()
            {
                Sender = sender,
                Data = data,
            }, out _);
        }

        public void Publish(EventDto @event)
        {
            this.Publish(@event, out _);
        }

        public void Confirm(EventDto @event)
        {
            if (!_published.TryGet(@event.Id, out PublishedEvent? published))
            {
                this.Publish(@event, out published);
            }

            _logger.Verbose("{ClassName}::{MethodName} - Confirming Event {EventId} {EventType}", nameof(EventPublishingService), nameof(Confirm), @event.Id.Value, @event.Data.GetType().Name);


            published.Status = PublishedEventStatus.Confirmed;
        }

        public void Confirm(VhId sender, IEventData data)
        {
            this.Confirm(new EventDto()
            {
                Sender = sender,
                Data = data
            });
        }

        public void Revert()
        {
            while(_published.TryPeek(out PublishedEvent? published) && published.Expired)
            {
                if(published.Status == PublishedEventStatus.Unconfirmed)
                {
                    this.Revert(published);
                }

                _published.TryDequeue(out _);
            }
        }

        public void Confirm()
        {
            _published.Clear();
        }


        private void Revert(PublishedEvent published)
        {
            if (!_publishers.TryGetValue(published.Event.Data.GetType(), out EventPublisher? publisher))
            {
                return;
            }

            publisher.Revert(published.Event);
            published.Status = PublishedEventStatus.Reverted;
        }

        private void Publish(EventDto @event, out PublishedEvent published)
        {
            published = new PublishedEvent(@event);
            if (!_published.TryEnqueue(@event.Id, published))
            {
                _logger.Warning("{ClassName}::{MethodName} - Unable to publish {EventName}, {EventId}; duplicate event.", nameof(EventPublishingService), nameof(Publish), @event.Data.GetType().Name, @event.Id.Value);
                return;
            }

            this.OnEvent?.Invoke(@event);

            if (!_publishers.TryGetValue(@event.Data.GetType(), out EventPublisher? publisher))
            {
                return;
            }

            publisher.Publish(@event);
        }
    }
}
