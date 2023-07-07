using Guppy.Resources;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Pieces.Utilities;

namespace VoidHuntersRevived.Common.Pieces.Resources
{
    public class Visible
    {
        public required Resource<Color> Color;
        public required Vector2[][] Shapes;
        public required Vector2[][] Paths;

        public static Visible Polygon(Resource<Color> color, int sides)
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
