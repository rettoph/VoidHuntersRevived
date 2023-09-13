using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
{
    [AutoLoad]
    public class SocketLocationsComponentSerializer : DoNotSerializeComponentSerializer<Sockets<Location>>
    {
    }
}
