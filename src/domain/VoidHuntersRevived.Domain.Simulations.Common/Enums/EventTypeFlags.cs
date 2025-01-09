namespace VoidHuntersRevived.Domain.Simulations.Common.Enums
{
    public enum EventTypeFlags
    {
        None = 0,

        /// <summary>
        /// When set, an event can be excecuted without lockstep verification within the predictive
        /// strategy.
        /// </summary>
        Predictive = 1
    }
}