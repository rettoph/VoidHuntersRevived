using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Domain.Simulations.Predictive.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Predictive
{
    internal class PredictedEvent
    {
        public static readonly TimeSpan Lifetime = TimeSpan.FromSeconds(5);

        private EventDto _event = null!;

        public EventDto Event
        {
            get => _event;
            set
            {
                _event = value;
                PublishedAt = DateTime.Now;
            }
        }
        public PredictedEventStatus Status { get; set; }
        public DateTime PublishedAt { get; private set; }

        public bool Expired => DateTime.Now - PublishedAt >= Lifetime;
    }
}
