using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common.Events
{
    public class Tree_Clean : IEventData
    {
        public bool IsPredictable => true;
        public bool IsPrivate { get; init; } = false;

        public required EntityGlobalId TreeGlobalId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Tree_Clean, VhId, EntityGlobalId>.Instance.Calculate(source, this.TreeGlobalId);
        }
    }
}