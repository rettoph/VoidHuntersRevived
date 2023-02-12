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
        private Rigid _component;
        private List<Shape> _shapes;

        public IEnumerable<Shape> Shapes
        {
            get => _shapes;
            set
            {
                _shapes = value.ToList();
                this.CleanComponent();
            }
        }

        public RigidConfiguration() : this(Array.Empty<Shape>())
        {

        }
        public RigidConfiguration(params Shape[] shapes)
        {
            _shapes = new List<Shape>(shapes);
            _component = default!;
        }

        public void Initialize(string path, IServiceProvider services)
        {
            this.CleanComponent();
        }

        public void AttachComponentToEntity(Entity entity)
        {
            entity.Attach<Rigid>(_component);
        }

        private void CleanComponent()
        {
            _component = new Rigid(this);
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
