namespace VoidHuntersRevived.Domain.Simulations.Common.Enums
{
    public enum EnqueueTickResponseEnum
    {
        /// <summary>
        /// Success response
        /// </summary>
        Enqueued,

        /// <summary>
        /// Failure response
        /// </summary>
        NotEnqueued,

        /// <summary>
        /// Indicates the exact tick is already enqueued
        /// </summary>
        DuplicateMatch,

        /// <summary>
        /// Inticates a tick with the same ID has already been enqueued
        /// but somehow the data is out of sync
        /// </summary>
        DuplicateMismatch
    }
}
