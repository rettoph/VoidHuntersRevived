using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class SoftDespawnEntity : IEventData
    {
        public bool IsPredictable => true;

        public required VhId VhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<SoftDespawnEntity, VhId, VhId>.Instance.Calculate(in source, this.VhId);
        }
    }
}
