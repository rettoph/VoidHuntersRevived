using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Pieces.Descriptors;

namespace VoidHuntersRevived.Game.Pieces
{
    public static class PieceTypes
    {
        public static EntityType<HullDescriptor> HullSquare = new(VoidHuntersRevivedGame.NameSpace, nameof(HullSquare));
        public static EntityType<HullDescriptor> HullTriangle = new(VoidHuntersRevivedGame.NameSpace, nameof(HullTriangle));
    }
}
