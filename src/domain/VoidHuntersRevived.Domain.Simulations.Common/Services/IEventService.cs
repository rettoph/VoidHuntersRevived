using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Services
{
    public interface IEventService
    {
        /// <summary>
        /// Publish an event
        /// </summary>
        /// <param name="event"></param>
        void Publish<TEvent>(TEvent @event)
            where TEvent : class, IStepEvent;


        /// <summary>
        /// Publish input event
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="data"></param>
        void Input<TInput>(TInput input)
            where TInput : class, IStepInput;


        /// <summary>
        /// <para>Enqueue an event to be published after the next <see cref="Step"/>.</para>
        /// <para><see cref="IStepEvent"/> instances may only be enqueued if <see cref="IStepEvent.IsPrivate"/> == false</para>
        /// </summary>
        /// <param name="event"></param>
        void Enqueue<TEvent>(IStepEvent @event)
            where TEvent : class, IStepEvent;
    }
}
