using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Messages
{
    internal class TickHistoryItem
    {
        public required Tick Tick { get; init; }
    }
}
