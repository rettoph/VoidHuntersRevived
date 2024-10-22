using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common;

namespace VoidHuntersRevived.Domain.Pieces
{
    public class BlueprintPiece(Key<IEntityTemplate> pieceTypeKey, IBlueprintPiece[][] children) : IBlueprintPiece
    {
        public Key<IEntityTemplate> PieceTypeKey { get; } = pieceTypeKey;
        public IBlueprintPiece[][] Children { get; } = children;
    }
}
