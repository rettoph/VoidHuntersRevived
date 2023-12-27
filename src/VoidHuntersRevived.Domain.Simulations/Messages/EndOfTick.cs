using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Simulations.Messages
{
    internal class EndOfTick : IEventData
    {
        public bool IsPredictable => false;
        public int TickId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<EndOfTick, VhId, int>.Instance.Calculate(source, this.TickId);
        }
    }
}
