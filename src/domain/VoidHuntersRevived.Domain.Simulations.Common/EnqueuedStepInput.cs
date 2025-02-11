using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public class EnqueuedStepInput : EnqueuedStepEvent
    {
        public EnqueuedStepInput(VhId sourceId, IStepInput data) : base(sourceId, data)
        {
        }

        public EnqueuedStepInput(Id<IStepEvent> id, IStepInput data) : base(id, data)
        {
        }
    }
}
