using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class HardDespawnEntity : IEventData
    {
        public required bool IsPrivate { get; init; }
        public required bool IsPredictable { get; init; }

        public required GlobalEntityId GlobalId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<HardDespawnEntity, VhId, GlobalEntityId, bool, bool>.Instance.Calculate(in source, this.GlobalId, this.IsPrivate, this.IsPredictable);
        }
    }
}
