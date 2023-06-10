﻿using Guppy.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Simulations
{
    public class EventDto
    {
        public required EventId Id { get; init; }
        public required IEventData Data { get; init; }
    }
}
