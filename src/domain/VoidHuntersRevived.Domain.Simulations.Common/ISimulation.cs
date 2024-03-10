using Autofac;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulation
    {
        VhId Id { get; }
        SimulationType Type { get; }
        ILifetimeScope Scope { get; }

        void Initialize(ISimulationService simulations);

        void Draw(GameTime realTime);

        void Update(GameTime realTime);

        /// <summary>
        /// Publish an event
        /// </summary>
        /// <param name="event"></param>
        void Publish(EventDto @event);

        /// <summary>
        /// Publish an event
        /// </summary>
        /// <param name="event"></param>
        void Publish(VhId sourceId, IEventData data);


        /// <summary>
        /// Publish input data
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
        void Enqueue(VhId sourceId, IEventData data);


        /// <summary>
        /// <para>Enqueue an event to be published after the next <see cref="Step"/>.</para>
        /// <para><see cref="IEventData"/> instances may only be enqueued if <see cref="IEventData.IsPrivate"/> == false</para>
        /// </summary>
        /// <param name="event"></param>
        void Enqueue(EventDto @event);
    }
}
