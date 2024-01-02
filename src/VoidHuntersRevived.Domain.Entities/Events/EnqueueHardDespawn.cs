using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Core.Utilities;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class EnqueueHardDespawn : IEventData
    {
        public bool IsPrivate => false;
        public bool IsPredictable => false;

        public required VhId VhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<EnqueueHardDespawn, VhId, VhId>.Instance.Calculate(in source, this.VhId);
        }
    }
}
