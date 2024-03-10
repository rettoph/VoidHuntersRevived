namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public interface IBlueprintPiece
    {
        PieceType PieceType { get; }

        IBlueprintPiece[][] Children { get; }
    }
}
