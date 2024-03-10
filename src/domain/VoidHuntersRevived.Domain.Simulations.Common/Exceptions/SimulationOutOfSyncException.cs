namespace VoidHuntersRevived.Domain.Simulations.Common.Exceptions
{
    public class SimulationOutOfSyncException : Exception
    {
        public SimulationOutOfSyncException(string message) : base(message) { }
        public SimulationOutOfSyncException(string message, Exception inner) : base(message, inner) { }
    }
}
