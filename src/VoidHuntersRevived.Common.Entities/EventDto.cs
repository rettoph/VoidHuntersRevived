using Guppy.Common;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Common.Entities
{
    public class EventDto
    {
        private VhId? _id;
        public VhId Id => _id ??= this.Data.CalculateHash(this.Sender);

        public required VhId Sender { get; init; }
        public required IEventData Data { get; init; }
    }
}
