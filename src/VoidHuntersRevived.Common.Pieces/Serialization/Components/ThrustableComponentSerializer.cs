﻿using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Pieces.Components.Instance;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
{
    [AutoLoad]
    internal class ThrustableComponentSerializer : DoNotSerializeComponentSerializer<Thrustable>
    {
    }
}
