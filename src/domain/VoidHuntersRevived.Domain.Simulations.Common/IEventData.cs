using VoidHuntersRevived.Common.Core;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface IEventData
    {
        /// <summary>
        /// Indicates an event is predictable and can be invoked before
        /// any lockstep confirmation is recieved.
        /// </summary>
        bool IsPredictable { get; }

        /// <summary>
        /// <para>
        /// Indicates an event that should only be invoked on the source
        /// simulation. It is not shared or confirmed by the predictive simulation.
        /// There is an implicit assumtion that syncing will happen despite the 
        /// unshared event.
        /// </para>
        /// <para>
        /// If false, the event may be enqueued with <see cref="ISimulation.Enqueue(EventDto)"/>
        /// </para>
        /// </summary>
        bool IsPrivate => false;

        VhId CalculateHash(in VhId source);
    }
}
