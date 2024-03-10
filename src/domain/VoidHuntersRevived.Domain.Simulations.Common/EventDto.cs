using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public class EventDto
    {
        private VhId? _id;
        public VhId Id => _id ??= this.Data.CalculateHash(this.SourceId);

        public required VhId SourceId { get; init; }
        public required IEventData Data { get; init; }
    }
}
