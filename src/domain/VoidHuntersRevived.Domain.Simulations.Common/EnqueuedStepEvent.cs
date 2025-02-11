using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public class EnqueuedStepEvent
    {
        public readonly Id<IStepEvent> Id;
        public readonly IStepEvent Data;

        public EnqueuedStepEvent(VhId sourceId, IStepEvent data)
        {
            this.Id = new(data.CalculateHash(sourceId));
            this.Data = data;
        }

        public EnqueuedStepEvent(Id<IStepEvent> id, IStepEvent data)
        {
            this.Id = id;
            this.Data = data;
        }
    }
}
