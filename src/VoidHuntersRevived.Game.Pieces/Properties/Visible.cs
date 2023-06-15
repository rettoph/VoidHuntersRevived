using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Game.Pieces.Utilities;

namespace VoidHuntersRevived.Game.Pieces.Properties
{
    public class Visible : IEntityProperty
    {
        public required string Color = string.Empty;
        public required Vector2[][] Shapes;
        public required Vector2[][] Paths;

        public static Visible Polygon(string color, int sides)
        {
            IEnumerable<PolygonHelper.VertexAngle> vertexAngles = PolygonHelper.CalculateVertexAngles(sides);

            return new Visible()
            {
                Color = color,
                Shapes = new[]
                {
                    vertexAngles.Select(x => x.XnaVertex).ToArray()
                },
                Paths = new[]
                {
                    vertexAngles.Select(x => x.XnaVertex).Concat(vertexAngles.First().XnaVertex.Yield()).ToArray()
                }
            };
        }
    }
}
