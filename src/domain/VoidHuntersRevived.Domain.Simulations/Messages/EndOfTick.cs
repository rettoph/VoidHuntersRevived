using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Core.Utilities;
using VoidHuntersRevived.Common.Simulations;

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
