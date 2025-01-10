using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class SoftSpawnEntity : IEventData
    {
        public required bool IsPrivate { get; init; }
        public bool IsPredictable => true;

        public required EntityGlobalId GlobalId { get; init; }

        public VhId CalculateHash(in VhId source) => HashBuilder<SoftSpawnEntity, VhId, EntityGlobalId>.Instance.Calculate(in source, this.GlobalId);
    }
}