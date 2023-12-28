using Serilog;
using System.Runtime.CompilerServices;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Simulations.Utilities
{
    public abstract class EventPublisher
    {
        public abstract void Publish(EventDto @event);
        public abstract void Revert(EventDto @event);

        public static Dictionary<Type, EventPublisher> BuildPublishers(IEngineService engines, ILogger logger)
        {
            Dictionary<Type, List<IEventEngine>> subscriptions = new();
            foreach (IEventEngine system in engines.OfType<IEventEngine>())
            {
                foreach (Type subscriberType in system.GetType().GetConstructedGenericTypes(typeof(IEventEngine<>)))
                {
                    if (!subscriptions.TryGetValue(subscriberType.GenericTypeArguments[0], out List<IEventEngine>? subSystems))
                    {
                        subscriptions[subscriberType.GenericTypeArguments[0]] = subSystems = new List<IEventEngine>();
                    }

                    subSystems.Add(system);
                }
            }

            Dictionary<Type, EventPublisher> publishers = new();
            foreach ((Type type, List<IEventEngine> subscribers) in subscriptions)
            {
                Type publisherType = typeof(EventPublisher<>).MakeGenericType(type);
                EventPublisher publisher = (EventPublisher)Activator.CreateInstance(publisherType, new object[] { logger, subscribers })!;
                publishers.Add(type, publisher);
            }

            return publishers;
        }
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
            _logger.Verbose("{ClassName}::{MethodName} - Publishing Event {EventId} {EventType}", nameof(EventPublisher), nameof(Publish), id.Value, typeof(T).Name);

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

            _logger.Verbose("{ClassName}::{MethodName} - Reverting Event {EventId} {EventType}", nameof(EventPublisher), nameof(Revert), id.Value, typeof(T).Name);
            foreach (IRevertEventEngine<T> reverter in _reverters)
            {
                reverter.Revert(id, data);
            }
        }
    }
}
