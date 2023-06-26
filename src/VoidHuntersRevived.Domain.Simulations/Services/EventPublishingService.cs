using Guppy.Common;
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

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public sealed class EventPublishingService
    {
        private readonly Dictionary<Type, SimulationEventPublisher> _publishers;

        public EventPublishingService(IEnumerable<IEventEngine> systems)
        {
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

        public void Revert(EventDto @event)
        {
            if (!_publishers.TryGetValue(@event.Data.GetType(), out SimulationEventPublisher? publisher))
            {
                return;
            }

            publisher.Revert(@event);
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

            public SimulationEventPublisher(List<IEventEngine> subscribers)
            {
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
                foreach (IRevertEventEngine<T> reverter in _reverters)
                {
                    reverter.Revert(id, data);
                }
            }
        }
    }
}
