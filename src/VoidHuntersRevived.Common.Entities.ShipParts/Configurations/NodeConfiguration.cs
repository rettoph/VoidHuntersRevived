using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Helpers;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Configurations
{
    public sealed class NodeConfiguration : IShipPartComponentConfiguration
    {
        public Vector2 Center { get; set; }
        public DegreeConfiguration[] Degrees { get; set; }

        public NodeConfiguration() : this(Vector2.Zero)
        {

        }
        public NodeConfiguration(Vector2 center, params DegreeConfiguration[] degrees)
        {
            this.Center = center;
            this.Degrees = degrees;
        }

        public void Initialize(string path, IServiceProvider services)
        {
            //
        }

        public void AttachComponentToEntity(Entity entity)
        {
            entity.Attach(new Node(this, entity.Id));
        }

        public static NodeConfiguration Polygon(int sides)
        {
            var vertexAngles = PolygonHelper.CalculateVertexAngles(sides).ToArray();
            var joints = new List<DegreeConfiguration>();

            var start = vertexAngles[0];
            var end = vertexAngles[1];

            for(int i=1; i<sides + 1; i++)
            {
                end = vertexAngles[i % vertexAngles.Length];

                joints.Add(new DegreeConfiguration(
                    rotation: end.Angle + MathHelper.PiOver2,
                    position: (end.Vertex + start.Vertex) / 2));

                start = end;
            }


            return new NodeConfiguration(
                center: joints.Select(x => x.Position).Average(),
                degrees: joints.ToArray());
        }
    }
}
