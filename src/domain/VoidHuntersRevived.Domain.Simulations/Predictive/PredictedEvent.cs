using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Predictive.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    public class PredictedEvent
    {
        public static readonly Fix64 Lifetime = (Fix64)5; // Represent 5 seconds

        public Id<IStepEvent> Id { get; private set; }
        public IStepEvent Event { get; private set; } = null!;
        public PredictedEventStatus Status { get; set; }
        public Fix64 PublishedAt { get; private set; }

        public void SetEvent(Id<IStepEvent> id, IStepEvent @event, Step currentStep)
        {
            this.Id = id;
            this.Event = @event;
            this.PublishedAt = currentStep.TotalTime;
        }

        public bool IsExpired(Step currentStep)
        {
            return currentStep.TotalTime - this.PublishedAt >= Lifetime;
        }
    }
}