using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Helpers;
using VoidHuntersRevived.Common.Physics;

namespace VoidHuntersRevived.Domain.Entities.ShipParts.Components
{
    public class Rigid : ShipPartComponent<Rigid>
    {
        public readonly Polygon[] Polygons;

        public Rigid(params Polygon[] polygons)
        {
            this.Polygons = polygons;
        }

        public static Rigid Polygon(Fix64 density, int sides)
        {
            Vertices vertices = new Vertices(PolygonHelper.CalculateVertexAngles(sides).Select(x => x.FixedVertex));
            Polygon shape = new Polygon(vertices, density);

            return new Rigid(shape);
        }

        public override ShipPartComponent Clone()
        {
            return this;
        }
    }
}
