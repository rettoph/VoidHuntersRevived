using Guppy.Attributes;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;

namespace VoidHuntersRevived.Domain.Pieces.Common.Serialization.Components
{
    [AutoLoad]
    internal sealed class BlueprintIdComponentSerializer : NotImplementedComponentSerializer<Id<Blueprint>>
    {
    }
}
