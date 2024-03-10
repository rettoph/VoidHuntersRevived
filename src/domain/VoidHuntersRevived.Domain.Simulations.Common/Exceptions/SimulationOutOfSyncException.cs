namespace VoidHuntersRevived.Common.Simulations.Exceptions
{
    public class SimulationOutOfSyncException : Exception
    {
        public SimulationOutOfSyncException(string message) : base(message) { }
        public SimulationOutOfSyncException(string message, Exception inner) : base(message, inner) { }
    }
}
