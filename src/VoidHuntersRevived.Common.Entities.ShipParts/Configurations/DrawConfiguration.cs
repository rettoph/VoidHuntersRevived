using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Helpers;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Configurations
{
    public sealed class DrawConfiguration : IShipPartComponentConfiguration
    {
        private Drawable _component = default!;

        public string Color { get; set; } = string.Empty;
        public Vector2[][] Shapes { get; set; } = Array.Empty<Vector2[]>();
        public Vector2[][] Paths { get; set; } = Array.Empty<Vector2[]>();

        public DrawConfiguration()
        {

        }
        public DrawConfiguration(string color, Vector2[][] shapes, Vector2[][] paths)
        {
            this.Color = color;
            this.Shapes = shapes;
            this.Paths = paths;
        }

        public void Initialize(string path, IServiceProvider services)
        {
            _component = new Drawable(this);
        }

        public void AttachComponentToEntity(Entity entity)
        {
            
            entity.Attach(_component);
        }

        public static DrawConfiguration Polygon(string color, int sides)
        {
            var vertexAngles = PolygonHelper.CalculateVertexAngles(sides);

            return new DrawConfiguration(
                color: color,
                shapes: new[]
                {
                    vertexAngles.Select(x => x.Vertex).ToArray()
                },
                paths: new[]
                {
                    vertexAngles.Select(x => x.Vertex).Concat(vertexAngles.First().Vertex.Yield()).ToArray()
                });
        }
    }
}
