using Guppy.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Ships.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Serialization.Components
{
    [AutoLoad]
    public sealed class HelmComponentSerializer : NotImplementedComponentSerializer<Helm>
    {
    }
}
