using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Game.Pieces.Utilities;

namespace VoidHuntersRevived.Game.Pieces.Resources
{
    public class Rigid
    {
        public required Polygon[] Shapes { get; init; }

        public static Rigid Polygon(Fix64 density, int sides)
        {
            Vertices vertices = new Vertices(PolygonHelper.CalculateVertexAngles(sides).Select(x => x.FixedVertex).ToArray());
            Polygon shape = new Polygon(vertices, density);

            Rigid rigid = new Rigid()
            {
                Shapes = new[]
                {
                    shape
                }
            };

            return rigid;
        }
    }
}
