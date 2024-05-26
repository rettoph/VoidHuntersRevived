using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public interface IBlueprintPiece
    {
        IEntityType<PieceDescriptor> PieceType { get; }

        IBlueprintPiece[][] Children { get; }
    }
}
