using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Predictive.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    internal class PredictedEvent
    {
        public static readonly Fix64 Lifetime = (Fix64)5; // Represent 5 seconds

        private EventDto _event = null!;

        public EventDto Event => _event;
        public PredictedEventStatus Status { get; set; }
        public Fix64 PublishedAt { get; private set; }

        public void SetEvent(EventDto @event, Step currentStep)
        {
            _event = @event;
            this.PublishedAt = currentStep.TotalTime;
        }

        public bool IsExpired(Step currentStep)
        {
            return currentStep.TotalTime - this.PublishedAt >= Lifetime;
        }
    }
}
