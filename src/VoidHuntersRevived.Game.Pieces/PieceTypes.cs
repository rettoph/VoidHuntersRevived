using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Game.Common;

namespace VoidHuntersRevived.Game.Pieces
{
    public class PieceTypes
    {
        public static EntityType Piece = new EntityType(VoidHuntersRevivedGame.NameSpace, nameof(Piece));
        public static EntityType Hull = new EntityType(VoidHuntersRevivedGame.NameSpace, nameof(Hull));
    }
}