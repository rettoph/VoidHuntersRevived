using FixedMath.NET;
using Guppy.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Game.Pieces
{
    public class PieceResources
    {
        public static class HullSquare
        {
            public static Resource<Sockets> Joints = new($"{nameof(HullSquare)}.{nameof(Joints)}");
        }

        public static class HullTriangle
        {
            public static Resource<Sockets> Joints = new($"{nameof(HullTriangle)}.{nameof(Joints)}");
        }
    }
}
