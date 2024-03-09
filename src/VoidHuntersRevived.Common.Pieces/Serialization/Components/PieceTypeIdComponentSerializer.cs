using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
{
    [AutoLoad]
    public sealed class PieceTypeIdComponentSerializer : DoNotSerializeComponentSerializer<Id<PieceType>>
    {
    }
}
