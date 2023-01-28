using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Common.ConvexHull;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Helpers;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Configurations
{
    public sealed class RigidConfiguration : IShipPartComponentConfiguration
    {
        private readonly Rigid _component;

        public readonly Shape[] Shapes;

        public RigidConfiguration(params Shape[] shapes)
        {
            _component = new Rigid(this);
            this.Shapes = shapes;
        }

        public void AttachComponentTo(Entity entity)
        {
            entity.Attach<Rigid>(_component);
        }

        public static RigidConfiguration Polygon(float density, int sides)
        {
            var vertices = new Vertices(PolygonHelper.CalculateVertexAngles(sides).Select(x => x.Vertex));
            vertices = GiftWrap.GetConvexHull(vertices);
            var shape = new PolygonShape(vertices, density);

            return new RigidConfiguration(shape);
        }
    }
}
