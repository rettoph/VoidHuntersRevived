using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class HardDespawnEntity : IEventData
    {
        public required bool IsPrivate { get; init; }
        public required bool IsPredictable { get; init; }

        public required VhId VhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<HardDespawnEntity, VhId, VhId, bool, bool>.Instance.Calculate(in source, this.VhId, this.IsPrivate, this.IsPredictable);
        }
    }
}
