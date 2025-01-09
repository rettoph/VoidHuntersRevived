using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public interface IBlueprintPiece
    {
        Key<IEntityTemplate> PieceTemplateKey { get; }

        IBlueprintPiece[][] Children { get; }
    }
}