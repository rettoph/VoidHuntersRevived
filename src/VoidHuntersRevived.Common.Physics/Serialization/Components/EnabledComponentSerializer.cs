using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Physics.Components;

namespace VoidHuntersRevived.Common.Physics.Serialization.Components
{
    [AutoLoad]
    public sealed class EnabledComponentSerializer : RawComponentSerializer<Enabled>
    {
    }
}
