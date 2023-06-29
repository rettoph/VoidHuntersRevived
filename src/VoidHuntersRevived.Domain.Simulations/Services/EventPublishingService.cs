﻿using Guppy.Common;
using Serilog;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public sealed class EventPublishingService
    {
        private readonly Dictionary<Type, SimulationEventPublisher> _publishers;
        private readonly HashCache<VhId> _publishedEvents;
        private readonly ILogger _logger;

        public EventPublishingService(ILogger logger, IEnumerable<IEventEngine> systems)
        {
            _logger = logger;
            _publishedEvents = new HashCache<VhId>(TimeSpan.FromSeconds(10));
            Dictionary<Type, List<IEventEngine>> subscriptions = new Dictionary<Type, List<IEventEngine>>();
            foreach (IEventEngine system in systems)
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

            _publishers = new Dictionary<Type, SimulationEventPublisher>();
            foreach ((Type type, List<IEventEngine> subscribers) in subscriptions)
            {
                Type publisherType = typeof(SimulationEventPublisher<>).MakeGenericType(type);
                SimulationEventPublisher publisher = (SimulationEventPublisher)Activator.CreateInstance(publisherType, new object[] { _logger, subscribers })!;
                _publishers.Add(type, publisher);
            }
        }

        public void Publish(EventDto @event)
        {
            if(_publishedEvents.Add(@event.Id) > 1)
            {
                _logger.Warning($"{nameof(SimulationEventPublisher)}::{nameof(Publish)} - Duplicate event '{@event.Data.GetType().Name}', '{@event.Id.Value}'");
                //return;
            }

            if(!_publishers.TryGetValue(@event.Data.GetType(), out SimulationEventPublisher? publisher))
            {
                return;
            }

            publisher.Publish(@event);
        }

        public void Revert(EventDto @event)
        {
            if (!_publishers.TryGetValue(@event.Data.GetType(), out SimulationEventPublisher? publisher))
            {
                return;
            }

            publisher.Revert(@event);
        }

        public void Clean()
        {
            _publishedEvents.Prune();
        }

        private abstract class SimulationEventPublisher
        {
            public abstract void Publish(EventDto @event);
            public abstract void Revert(EventDto @event);
        }
        private class SimulationEventPublisher<T> : SimulationEventPublisher
            where T : class, IEventData
        {
            private readonly IEventEngine<T>[] _subscribers;
            private readonly IRevertEventEngine<T>[] _reverters;
            private readonly ILogger _logger;

            public SimulationEventPublisher(ILogger logger, List<IEventEngine> subscribers)
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
                if(_reverters.Length == 0)
                {
                    return;
                }

                _logger.Verbose($"{nameof(SimulationEventPublisher)}::{nameof(Revert)} - Reverting '{typeof(T).Name}', '{id.Value}'");
                foreach (IRevertEventEngine<T> reverter in _reverters)
                {
                    reverter.Revert(id, data);
                }
            }
        }
    }
}
