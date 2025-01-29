using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Messages
{
    public class TickHistoryItem
    {
        public required Tick Tick { get; init; }
    }
}