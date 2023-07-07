using FixedMath.NET;
using Guppy.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Resources;

namespace VoidHuntersRevived.Game.Pieces
{
    public class PieceResources
    {
        public static class HullSquare
        {
            public static Resource<Rigid> Rigid = new($"{nameof(HullSquare)}.{nameof(Rigid)}");
            public static Resource<Visible> Visible = new($"{nameof(HullSquare)}.{nameof(Visible)}");
        }

        public static class HullTriangle
        {
            public static Resource<Rigid> Rigid = new($"{nameof(HullTriangle)}.{nameof(Rigid)}");
            public static Resource<Visible> Visible = new($"{nameof(HullTriangle)}.{nameof(Visible)}");
        }
    }
}
