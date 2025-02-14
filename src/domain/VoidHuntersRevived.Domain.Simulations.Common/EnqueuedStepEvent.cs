using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;

namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public class EnqueuedStepEvent
    {
        public readonly Id<IStepEvent> Id;
        public readonly IStepEvent Data;

        public EnqueuedStepEvent(VhId sourceId, IStepEvent data)
        {
            this.Id = data.CalculateId(sourceId);
            this.Data = data;
        }

        public EnqueuedStepEvent(Id<IStepEvent> id, IStepEvent data)
        {
            this.Id = id;
            this.Data = data;
        }
    }
}
