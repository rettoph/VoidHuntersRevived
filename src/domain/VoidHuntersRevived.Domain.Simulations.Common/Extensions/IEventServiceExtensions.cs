using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Common.Extensions
{
    public static class IEventServiceExtensions
    {
        /// <summary>
        /// Publish an event
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="event"></param>
        public static void Publish(this IStepEventService eventService, VhId sourceId, IStepEvent @event)
        {
            eventService.Publish(@event.CalculateId(sourceId), @event);
        }

        /// <summary>
        /// <para>Enqueue an event to be published after the next <see cref="Step"/>.</para>
        /// <para><see cref="IStepEvent"/> instances may only be enqueued if <see cref="IStepEvent.IsPrivate"/> == false</para>
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="sourceId"></param>
        /// <param name="event"></param>
        public static void Enqueue(this IStepEventService eventService, VhId sourceId, IStepEvent @event)
        {
            eventService.Enqueue(new EnqueuedStepEvent(sourceId, @event));
        }

        /// <summary>
        /// Publish input event. Should never be called directly - use
        /// <see cref="ISimulation.Input(VhId, IStepInput)"/>
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="event"></param>
        public static void Input(this IStepEventService eventService, VhId sourceId, IStepInput input)
        {
            eventService.Input(new EnqueuedStepInput(sourceId, input));
        }
    }
}
