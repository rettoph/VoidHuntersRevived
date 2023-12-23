namespace VoidHuntersRevived.Common.Pieces
{
    public interface IBlueprintPiece
    {
        Piece Piece { get; }

        IBlueprintPiece[][] Children { get; }
    }
}
