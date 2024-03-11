using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;

namespace VoidHuntersRevived.Domain.Pieces.Common.Serialization.Components
{
    [AutoLoad]
    public sealed class PieceTypeIdComponentSerializer : DoNotSerializeComponentSerializer<Id<PieceType>>
    {
    }
}
