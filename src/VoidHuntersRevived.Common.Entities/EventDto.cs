using Guppy.Common;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Common.Entities
{
    public class EventDto
    {
        public required VhId Id { get; init; }
        public required IEventData Data { get; init; }
    }
}
