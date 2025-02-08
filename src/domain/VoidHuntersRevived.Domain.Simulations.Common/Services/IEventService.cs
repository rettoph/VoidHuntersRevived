using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Services
{
    public interface IEventService
    {
        /// <summary>
        /// Publish an event
        /// </summary>
        /// <param name="event"></param>
        void Publish(EventDto @event);

        /// <summary>
        /// Publish an event
        /// </summary>
        /// <param name="event"></param>
        void Publish(VhId sourceId, IEventData data)
        {
            this.Publish(new EventDto()
            {
                SourceId = sourceId,
                Data = data
            });
        }

        /// <summary>
        /// Publish input event
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="data"></param>
        void Input(VhId sourceId, IInputData data);

        /// <summary>
        /// <para>Enqueue an event to be published after the next <see cref="Step"/>.</para>
        /// <para><see cref="IEventData"/> instances may only be enqueued if <see cref="IEventData.IsPrivate"/> == false</para>
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="data"></param>
        void Enqueue(VhId sourceId, IEventData data)
        {
            this.Enqueue(new EventDto()
            {
                SourceId = sourceId,
                Data = data
            });
        }

        /// <summary>
        /// <para>Enqueue an event to be published after the next <see cref="Step"/>.</para>
        /// <para><see cref="IEventData"/> instances may only be enqueued if <see cref="IEventData.IsPrivate"/> == false</para>
        /// </summary>
        /// <param name="event"></param>
        void Enqueue(EventDto @event);
    }
}
