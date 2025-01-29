using System.Runtime.CompilerServices;
using Guppy.Core.Common.Extensions.System;
using Guppy.Core.Logging.Common;
using Guppy.Core.Logging.Common.Services;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Simulations.Utilities
{
    public abstract class EventPublisher
    {
        public abstract void Publish(EventDto @event);
        public abstract void Revert(EventDto @event);

        public static void PopulatePublishers(
            IEngineService engineService,
            ILoggerService loggerProvider,
            Dictionary<Type, EventPublisher> publishers)
        {
            Dictionary<Type, List<IEventEngine>> subscriptions = [];
            foreach (IEventEngine system in engineService.OfType<IEventEngine>())
            {
                foreach (Type subscriberType in system.GetType().GetConstructedGenericTypes(typeof(IEventEngine<>)))
                {
                    if (!subscriptions.TryGetValue(subscriberType.GenericTypeArguments[0], out List<IEventEngine>? subSystems))
                    {
                        subscriptions[subscriberType.GenericTypeArguments[0]] = subSystems = [];
                    }

                    subSystems.Add(system);
                }
            }

            foreach ((Type type, List<IEventEngine> subscribers) in subscriptions)
            {
                Type publisherType = typeof(EventPublisher<>).MakeGenericType(type);
                EventPublisher publisher = (EventPublisher)Activator.CreateInstance(publisherType, [loggerProvider.GetLogger(publisherType), subscribers])!;
                publishers.Add(type, publisher);
            }
        }

        public static Dictionary<Type, EventPublisher> BuildPublishers(IEngineService engines, ILoggerService loggerService)
        {
            Dictionary<Type, EventPublisher> publishers = [];

            EventPublisher.PopulatePublishers(engines, loggerService, publishers);

            return publishers;
        }
    }
    internal class EventPublisher<T>(ILogger logger, List<IEventEngine> subscribers) : EventPublisher
        where T : class, IEventData
    {
        private readonly IEventEngine<T>[] _subscribers = subscribers.OfType<IEventEngine<T>>().ToArray();
        private readonly IRevertEventEngine<T>[] _reverters = subscribers.OfType<IRevertEventEngine<T>>().ToArray();
        private readonly ILogger _logger = logger;

        public override void Publish(EventDto @event)
        {
            this.Publish(@event.Id, Unsafe.As<T>(@event.Data));
        }

        private void Publish(in VhId id, T data)
        {
            this._logger.Verbose("Publishing Event {EventId} {EventType}", id.Value, typeof(T).Name);

            foreach (IEventEngine<T> subscriber in this._subscribers)
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
            if (this._reverters.Length == 0)
            {
                return;
            }

            this._logger.Verbose("Reverting Event {EventId} {EventType}", id.Value, typeof(T).Name);
            foreach (IRevertEventEngine<T> reverter in this._reverters)
            {
                reverter.Revert(id, data);
            }
        }
    }
}