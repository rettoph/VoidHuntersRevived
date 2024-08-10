using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public interface IBlueprintPiece
    {
        IKey<IEntityType> PieceTypeKey { get; }

        IBlueprintPiece[][] Children { get; }
    }
}
