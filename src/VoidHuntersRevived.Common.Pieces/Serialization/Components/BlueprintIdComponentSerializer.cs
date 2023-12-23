using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
{
    [AutoLoad]
    internal sealed class BlueprintIdComponentSerializer : NotImplementedComponentSerializer<Id<BlueprintDto>>
    {
    }
}
