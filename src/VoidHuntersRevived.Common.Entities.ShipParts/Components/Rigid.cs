using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common.Entities.ShipParts.Helpers;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public class Rigid : ShipPartComponent<Rigid>
    {
        public readonly Shape[] Shapes;

        public Rigid(params Shape[] shapes)
        {
            this.Shapes = shapes;
        }

        public static Rigid Polygon(float density, int sides)
        {
            var vertices = new Vertices(PolygonHelper.CalculateVertexAngles(sides).Select(x => x.Vertex));
            vertices = GiftWrap.GetConvexHull(vertices);
            var shape = new PolygonShape(vertices, density);

            return new Rigid(shape);
        }

        public override ShipPartComponent Clone()
        {
            return this;
        }
    }
}
