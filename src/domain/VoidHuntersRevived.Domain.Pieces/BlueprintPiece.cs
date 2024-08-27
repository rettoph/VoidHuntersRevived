using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common;

namespace VoidHuntersRevived.Domain.Pieces
{
    public class BlueprintPiece : IBlueprintPiece
    {
        public Key<IEntityType> PieceTypeKey { get; }
        public IBlueprintPiece[][] Children { get; }

        public BlueprintPiece(Key<IEntityType> pieceTypeKey, IBlueprintPiece[][] children)
        {
            this.PieceTypeKey = pieceTypeKey;
            this.Children = children;
        }
    }
}
