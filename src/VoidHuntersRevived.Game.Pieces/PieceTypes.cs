using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Game.Common;

namespace VoidHuntersRevived.Game.Pieces
{
    public static class PieceTypes
    {
        public static EntityType<HullDescriptor> HullSquare = new(nameof(HullSquare));
        public static EntityType<HullDescriptor> HullTriangle = new(nameof(HullTriangle));
    }
}
