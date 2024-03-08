namespace VoidHuntersRevived.Common.Pieces
{
    public interface IBlueprintPiece
    {
        PieceType PieceType { get; }

        IBlueprintPiece[][] Children { get; }
    }
}
