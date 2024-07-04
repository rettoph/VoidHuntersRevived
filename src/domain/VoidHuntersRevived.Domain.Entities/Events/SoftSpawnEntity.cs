using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class SoftSpawnEntity : IEventData
    {
        public bool IsPrivate => true;
        public bool IsPredictable => true;

        public required VhId VhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<SoftSpawnEntity, VhId, VhId>.Instance.Calculate(in source, this.VhId);
        }
    }
}
