using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class HardDespawnEntity : IEventData
    {
        public bool IsPrivate => false;
        public bool IsPredictable => false;

        public required VhId VhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<HardDespawnEntity, VhId, VhId>.Instance.Calculate(in source, this.VhId);
        }
    }
}
