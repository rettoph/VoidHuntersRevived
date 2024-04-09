using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public interface IBlueprintPiece
    {
        EntityContext PieceType { get; }

        IBlueprintPiece[][] Children { get; }
    }
}
