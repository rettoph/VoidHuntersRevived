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
        /// Indicates an event that should only be invoked on the source
        /// simulation. It is not shared or confirmed by the predictive simulation.
        /// There is an implicit assumtion that syncing will happen despite the 
        /// unshared event
        /// </summary>
        bool IsPrivate => false;

        VhId CalculateHash(in VhId source);
    }
}
