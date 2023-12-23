﻿using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Ships.Components;

namespace VoidHuntersRevived.Common.Ships.Serialization.Components
{
    [AutoLoad]
    public sealed class TractorableComponentSerializer : RawComponentSerializer<Tractorable>
    {
    }
}
