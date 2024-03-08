using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Pieces
{
    public interface IBlueprintPiece
    {
        Id<IBlueprintPiece> Id { get; }

        PieceType PieceType { get; }

        IBlueprintPiece[][] Children { get; }
    }
}
