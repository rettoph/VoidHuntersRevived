using Guppy.Common;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Common.Events
{
    public class EventDto
    {
        public required VhId Id { get; init; }
        public required IEventData Data { get; init; }
    }
}
