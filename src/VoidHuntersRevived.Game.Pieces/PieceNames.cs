using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Game.Common;

namespace VoidHuntersRevived.Game.Pieces
{
    public static class PieceNames
    {
        public static EntityName HullSquare = new EntityName(VoidHuntersRevivedGame.NameSpace, nameof(HullSquare));
        public static EntityName HullTriangle = new EntityName(VoidHuntersRevivedGame.NameSpace, nameof(HullTriangle));
    }
}
