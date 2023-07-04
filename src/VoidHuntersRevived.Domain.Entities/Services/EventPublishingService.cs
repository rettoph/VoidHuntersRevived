using Guppy.Common;
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
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public sealed class EventPublishingService : IEventPublishingService, IEnginesGroupEngine
    {
        private Dictionary<Type, EventPublisher> _publishers;
        private Dictionary<VhId, EventDto> _unverifiedEvents;
        private readonly ILogger _logger;

        public event OnEventDelegate<EventDto>? OnVerifiedEvent;

        public EventPublishingService(ILogger logger)
        {
            _logger = logger;
            _publishers = null!;
            _unverifiedEvents = new Dictionary<VhId, EventDto>();
        }

        public void Initialize(IEngineService engines)
        {
            Dictionary<Type, List<IEventEngine>> subscriptions = new Dictionary<Type, List<IEventEngine>>();
            foreach (IEventEngine system in engines.OfType<IEventEngine>())
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

        public void Publish(EventDto @event, EventValidity validity)
        {
            if (!_publishers.TryGetValue(@event.Data.GetType(), out EventPublisher? publisher))
            {
                return;
            }

            if (validity == EventValidity.Valid)
            {
                this.OnVerifiedEvent?.Invoke(@event);
                publisher.Publish(@event);
                publisher.Validate(@event);
            }
            else
            {
                publisher.Publish(@event);
                _unverifiedEvents.Add(@event.Id, @event);
            }
        }

        public void Revert(VhId eventId)
        {
            if(!_unverifiedEvents.Remove(eventId, out EventDto? @event))
            {
                return;
            }

            if (!_publishers.TryGetValue(@event.Data.GetType(), out EventPublisher? publisher))
            {
                return;
            }

            publisher.Revert(@event);
        }

        public void Validate(VhId eventId)
        {
            if (!_unverifiedEvents.Remove(eventId, out EventDto? @event))
            {
                return;
            }

            if (!_publishers.TryGetValue(@event.Data.GetType(), out EventPublisher? publisher))
            {
                return;
            }

            publisher.Validate(@event);
            this.OnVerifiedEvent?.Invoke(@event);
        }
    }
}
