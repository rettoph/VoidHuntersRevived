using Microsoft.Xna.Framework;
using System.Linq;
using VoidHuntersRevived.Common.Entities.ShipParts.Helpers;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public class Drawable : ShipPartComponent<Drawable>
    {
        public string Color { get; set; } = string.Empty;
        public Vector2[][] Shapes { get; set; } = Array.Empty<Vector2[]>();
        public Vector2[][] Paths { get; set; } = Array.Empty<Vector2[]>();

        public Drawable(string color, Vector2[][] shapes, Vector2[][] paths)
        {
            this.Color = color;
            this.Shapes = shapes;
            this.Paths = paths;
        }

        public static Drawable Polygon(string color, int sides)
        {
            IEnumerable<PolygonHelper.VertexAngle> vertexAngles = PolygonHelper.CalculateVertexAngles(sides);

            return new Drawable(
                color: color,
                shapes: new[]
                {
                    vertexAngles.Select(x => x.XnaVertex).ToArray()
                },
                paths: new[]
                {
                    vertexAngles.Select(x => x.XnaVertex).Concat(vertexAngles.First().XnaVertex.Yield()).ToArray()
                });
        }

        public override ShipPartComponent Clone()
        {
            return this;
        }
    }
}
