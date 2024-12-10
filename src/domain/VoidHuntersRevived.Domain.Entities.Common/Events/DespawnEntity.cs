using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Events
{
    public sealed class DespawnEntity : IEventData
    {
        public required bool IsPrivate { get; init; }

        public bool IsPredictable => true;

        public required EntityGlobalId GlobalId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<DespawnEntity, VhId, EntityGlobalId>.Instance.Calculate(in source, this.GlobalId);
        }
    }
}
