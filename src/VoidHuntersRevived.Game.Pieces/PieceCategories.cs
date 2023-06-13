using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Game.Common;

namespace VoidHuntersRevived.Game.Pieces
{
    public class PieceCategories
    {
        public static PieceCategory Piece = new PieceCategory(VoidHuntersRevivedGame.NameSpace, nameof(Piece));
        public static PieceCategory Hull = new PieceCategory(VoidHuntersRevivedGame.NameSpace, nameof(Hull));
    }
}