using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Simulations.Messages
{
    public class TickHistoryItem
    {
        public required Tick Tick { get; init; }
    }
}