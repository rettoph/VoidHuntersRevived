using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Game.Pieces.Utilities;

namespace VoidHuntersRevived.Game.Pieces.Properties
{
    public class Rigid : IEntityProperty
    {
        public required Polygon[] Polygons { get; init; }

        public static Rigid Polygon(Fix64 density, int sides)
        {
            Vertices vertices = new Vertices(PolygonHelper.CalculateVertexAngles(sides).Select(x => x.FixedVertex));
            Polygon shape = new Polygon(vertices, density);

            return new Rigid()
            {
                Polygons = new[] { shape }
            };
        }
    }
}
