using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components.Instance;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
{
    [AutoLoad]
    public class SocketLocationsComponentSerializer : DoNotSerializeComponentSerializer<Sockets<Location>>
    {
    }
}
