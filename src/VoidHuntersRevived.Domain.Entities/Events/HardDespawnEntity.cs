using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class HardDespawnEntity : IEventData
    {
        public bool IsPredictable => false;

        public required VhId VhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<HardDespawnEntity, VhId, VhId>.Instance.Calculate(in source, this.VhId);
        }
    }
}
