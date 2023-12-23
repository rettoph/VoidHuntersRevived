using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Physics.Components;

namespace VoidHuntersRevived.Common.Physics.Serialization.Components
{
    [AutoLoad]
    public sealed class CollisionComponentSerializer : RawComponentSerializer<Collision>
    {
    }
}
