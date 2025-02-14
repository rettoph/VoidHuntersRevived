using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public abstract class BaseEventService(IMessageBus messageBus, ILogger logger) : IEventService
    {
        private readonly Queue<EnqueuedStepEvent> _enqueued = [];
        protected readonly ILogger logger = logger;
        protected IMessageBus messageBus { get; } = messageBus;

        public event OnEventDelegate<Id<IStepEvent>, IStepEvent>? OnEvent;

        public abstract void Input(EnqueuedStepInput input);

        public void Enqueue(EnqueuedStepEvent @event)
        {
            this.logger.Verbose("Enqueing {EventName}, {EventId}", @event.Data.GetType().Name, @event.Id);
            this._enqueued.Enqueue(@event);
        }

        public void Flush()
        {
            while (this._enqueued.TryDequeue(out EnqueuedStepEvent? enqueued))
            {
                this.logger.Verbose("Publishing Enqueued {EventName}, {EventId}", enqueued.Data.GetType().Name, enqueued.Id);
                enqueued.Data.Publish(enqueued.Id.Value, this.messageBus);
            }
        }

        public virtual void Publish(Id<IStepEvent> id, IStepEvent @event)
        {
            this.logger.Verbose("Publishing {EventName}, {EventId}", @event.GetType().Name, id);

            this.OnEvent?.Invoke(id, @event);

            @event.Publish(id.Value, this.messageBus);
        }
    }
}
