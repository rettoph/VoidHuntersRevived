using Guppy.Game.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public interface IStrategy : IScene, IDisposable
    {
        StrategyTypeEnum Type { get; }
        ISimulation Simulation { get; }

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