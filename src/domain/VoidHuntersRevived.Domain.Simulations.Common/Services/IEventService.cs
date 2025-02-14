using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Services
{
    public interface IEventService
    {
        /// <summary>
        /// Invoked every time an event is published
        /// </summary>
        event OnEventDelegate<Id<IStepEvent>, IStepEvent>? OnEvent;

        /// <summary>
        /// Publish an event
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="sourceId"></param>
        /// <param name="event"></param>
        void Publish(Id<IStepEvent> id, IStepEvent @event);

        /// <summary>
        /// Publish an event
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="event"></param>
        void Publish(VhId sourceId, IStepEvent @event)
        {
            this.Publish(new Id<IStepEvent>(@event.CalculateHash(sourceId)), @event);
        }

        /// <summary>
        /// <para>Enqueue an event to be published after the next <see cref="Step"/>.</para>
        /// <para><see cref="IStepEvent"/> instances may only be enqueued if <see cref="IStepEvent.IsPrivate"/> == false</para>
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="sourceId"></param>
        /// <param name="event"></param>
        void Enqueue(VhId sourceId, IStepEvent @event)
        {
            this.Enqueue(new EnqueuedStepEvent(sourceId, @event));
        }

        /// <summary>
        /// <para>Enqueue an event to be published after the next <see cref="Step"/>.</para>
        /// <para><see cref="IStepEvent"/> instances may only be enqueued if <see cref="IStepEvent.IsPrivate"/> == false</para>
        /// </summary>
        /// <param name="event"></param>
        void Enqueue(EnqueuedStepEvent @event);

        /// <summary>
        /// Publish any enqueued events
        /// </summary>
        void Flush();

        /// <summary>
        /// Publish input event. Should never be called directly - use
        /// <see cref="ISimulation.Input(VhId, IStepInput)"/>
        /// </summary>
        /// <param name="input"></param>
        void Input(EnqueuedStepInput input);

        /// <summary>
        /// Publish input event. Should never be called directly - use
        /// <see cref="ISimulation.Input(VhId, IStepInput)"/>
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="event"></param>
        void Input(VhId sourceId, IStepInput input)
        {
            this.Input(new EnqueuedStepInput(sourceId, input));
        }
    }
}
