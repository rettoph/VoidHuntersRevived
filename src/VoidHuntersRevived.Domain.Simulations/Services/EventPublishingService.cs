using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public sealed class EventPublishingService
    {
        private readonly Dictionary<Type, SimulationEventPublisher> _publishers;

        public EventPublishingService(IEnumerable<ISystem> systems)
        {
            Dictionary<Type, List<ISystem>> subscriptions = new Dictionary<Type, List<ISystem>>();
            foreach (ISystem system in systems)
            {
                foreach (Type subscriberType in system.GetType().GetConstructedGenericTypes(typeof(IEventSubscriber<>)))
                {
                    if (!subscriptions.TryGetValue(subscriberType.GenericTypeArguments[0], out List<ISystem>? subSystems))
                    {
                        subscriptions[subscriberType.GenericTypeArguments[0]] = subSystems = new List<ISystem>();
                    }

                    subSystems.Add(system); ;
                }
            }

            _publishers = new Dictionary<Type, SimulationEventPublisher>();
            foreach ((Type type, List<ISystem> subscribers) in subscriptions)
            {
                Type publisherType = typeof(SimulationEventPublisher<>).MakeGenericType(type);
                SimulationEventPublisher publisher = (SimulationEventPublisher)Activator.CreateInstance(publisherType, new[] { subscribers })!;
                _publishers.Add(type, publisher);
            }
        }

        public void Publish(EventDto @event)
        {
            if(!_publishers.TryGetValue(@event.Data.GetType(), out SimulationEventPublisher? publisher))
            {
                return;
            }

            publisher.Publish(@event);
        }

        private abstract class SimulationEventPublisher
        {
            public abstract void Publish(EventDto @event);
        }
        private class SimulationEventPublisher<T> : SimulationEventPublisher
            where T : class, IEventData
        {
            private readonly IEventSubscriber<T>[] _subscribers;

            public SimulationEventPublisher(List<ISystem> subscribers)
            {
                _subscribers = subscribers.OfType<IEventSubscriber<T>>().ToArray();
            }

            public override void Publish(EventDto @event)
            {
                this.Invoke(@event.Id, Unsafe.As<T>(@event.Data));
            }

            private void Invoke(in EventId id, T data)
            {
                foreach(IEventSubscriber<T> subscriber in _subscribers)
                {
                    subscriber.Process(in id, data);
                }
            }
        }
    }
}
