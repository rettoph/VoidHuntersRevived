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

namespace VoidHuntersRevived.Domain.Entities.Utilities
{
    internal abstract class EventPublisher
    {
        public abstract void Publish(EventDto @event);
        public abstract void Revert(EventDto @event);
        public abstract void Validate(EventDto @event);
    }
    internal class EventPublisher<T> : EventPublisher
        where T : class, IEventData
    {
        private readonly IEventEngine<T>[] _subscribers;
        private readonly IRevertEventEngine<T>[] _reverters;
        private readonly IVerifyEventEngine<T>[] _verifiers;
        private readonly ILogger _logger;

        public EventPublisher(ILogger logger, List<IEventEngine> subscribers)
        {
            _logger = logger;
            _subscribers = subscribers.OfType<IEventEngine<T>>().ToArray();
            _reverters = subscribers.OfType<IRevertEventEngine<T>>().ToArray();
            _verifiers = subscribers.OfType<IVerifyEventEngine<T>>().ToArray();
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

        public override void Validate(EventDto @event)
        {
            this.Validate(@event.Id, Unsafe.As<T>(@event.Data));
        }

        private void Revert(in VhId id, T data)
        {
            if (_reverters.Length == 0)
            {
                return;
            }

            _logger.Verbose($"{nameof(EventPublisher)}::{nameof(Revert)} - Reverting '{typeof(T).Name}', '{id.Value}'");
            foreach (IRevertEventEngine<T> reverter in _reverters)
            {
                reverter.Revert(id, data);
            }
        }

        private void Validate(in VhId id, T data)
        {
            if (_verifiers.Length == 0)
            {
                return;
            }

            _logger.Verbose($"{nameof(EventPublisher)}::{nameof(Revert)} - Verifying '{typeof(T).Name}', '{id.Value}'");
            foreach (IVerifyEventEngine<T> verify in _verifiers)
            {
                verify.Validate(id, data);
            }
        }
    }
}
