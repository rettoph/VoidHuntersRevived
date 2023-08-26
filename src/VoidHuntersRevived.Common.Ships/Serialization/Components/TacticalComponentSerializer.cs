using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Ships.Components;

namespace VoidHuntersRevived.Common.Ships.Serialization.Components
{
    [AutoLoad]
    public sealed class TacticalComponentSerializer : RawComponentSerializer<Tactical>
    {
    }
}
