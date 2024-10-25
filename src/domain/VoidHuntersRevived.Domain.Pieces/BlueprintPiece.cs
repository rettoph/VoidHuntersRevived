using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common;

namespace VoidHuntersRevived.Domain.Pieces
{
    public class BlueprintPiece(Key<IEntityTemplate> pieceTemplateKey, IBlueprintPiece[][] children) : IBlueprintPiece
    {
        public Key<IEntityTemplate> PieceTemplateKey { get; } = pieceTemplateKey;
        public IBlueprintPiece[][] Children { get; } = children;
    }
}
