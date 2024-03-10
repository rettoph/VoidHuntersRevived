using Guppy.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Physics.Common.Components;

namespace VoidHuntersRevived.Domain.Physics.Common.Serialization.Components
{
    [AutoLoad]
    public sealed class CollisionComponentSerializer : RawComponentSerializer<Collision>
    {
    }
}
