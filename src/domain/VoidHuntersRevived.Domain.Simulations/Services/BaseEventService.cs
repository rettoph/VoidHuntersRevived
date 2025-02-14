using Guppy.Core.Messaging.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public abstract class BaseEventService(IMessageBus messageBus) : IEventService
    {
        private readonly Queue<EnqueuedStepEvent> _enqueued = [];

        protected IMessageBus messageBus { get; } = messageBus;

        public event OnEventDelegate<Id<IStepEvent>, IStepEvent>? OnEvent;

        public abstract void Input(EnqueuedStepInput input);

        public void Enqueue(EnqueuedStepEvent @event)
        {
            this._enqueued.Enqueue(@event);
        }

        public void Flush()
        {
            while (this._enqueued.TryDequeue(out EnqueuedStepEvent? enqueued))
            {
                enqueued.Data.Publish(enqueued.Id.Value, this.messageBus);
            }
        }

        public virtual void Publish(Id<IStepEvent> id, IStepEvent @event)
        {
            this.OnEvent?.Invoke(id, @event);

            @event.Publish(id.Value, this.messageBus);
        }
    }
}
